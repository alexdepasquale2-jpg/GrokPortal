const MAX = 48;

const BUTTON = {
  stealth: "TAKE DOWN",
  finish: "CHOKE",
  brawl: "COMBO",
  combo: "COMBO",
  body: "DRAG",
  haul: "DROP",
};

export function mountTouch(input, game, canvas) {
  const root = document.getElementById("touch");
  const stick = document.getElementById("stick");
  const knob = document.getElementById("knob");
  const params = new URLSearchParams(location.search);
  const force = params.has("touch");

  const enable = () => {
    root.hidden = false;
    game.touch = true;
    if (!params.has("dev")) game.dev.on = false;
  };

  if (force || window.matchMedia("(pointer: coarse)").matches) enable();
  window.addEventListener("touchstart", enable, { once: true, passive: true });

  canvas.addEventListener(
    "pointerdown",
    (e) => {
      if (e.target.closest("#touch .stick, #touch .btns")) return;
      if (game.status !== "play") {
        e.preventDefault();
        input.tap("Space");
      }
    },
    { passive: false }
  );

  let stickId = null;
  const origin = { x: 0, y: 0 };

  const setStick = (e) => {
    const dx = e.clientX - origin.x;
    const dy = e.clientY - origin.y;
    const len = Math.hypot(dx, dy);
    const capped = Math.min(len, MAX);
    const nx = len > 0 ? dx / len : 0;
    const ny = len > 0 ? dy / len : 0;
    input.stick.x = (nx * capped) / MAX;
    input.stick.y = (ny * capped) / MAX;
    knob.style.transform = `translate(${nx * capped}px, ${ny * capped}px)`;
  };

  const endStick = (e) => {
    if (stickId == null || e.pointerId !== stickId) return;
    stickId = null;
    input.stick.x = 0;
    input.stick.y = 0;
    knob.style.transform = "translate(0, 0)";
  };

  stick.addEventListener(
    "pointerdown",
    (e) => {
      e.preventDefault();
      stickId = e.pointerId;
      stick.setPointerCapture(e.pointerId);
      const r = stick.getBoundingClientRect();
      origin.x = r.left + r.width / 2;
      origin.y = r.top + r.height / 2;
      setStick(e);
    },
    { passive: false }
  );
  stick.addEventListener("pointermove", (e) => {
    if (e.pointerId === stickId) setStick(e);
  });
  stick.addEventListener("pointerup", endStick);
  stick.addEventListener("pointercancel", endStick);

  const bind = (id, code) => {
    const el = document.getElementById(id);
    el.addEventListener(
      "pointerdown",
      (e) => {
        e.preventDefault();
        e.stopPropagation();
        el.classList.add("hot");
        input.tap(code);
        if (navigator.vibrate) navigator.vibrate(8);
      },
      { passive: false }
    );
    const cool = () => el.classList.remove("hot");
    el.addEventListener("pointerup", cool);
    el.addEventListener("pointercancel", cool);
    el.addEventListener("pointerleave", cool);
  };

  bind("btn-lantern", "TouchLantern");
  bind("btn-dash", "ShiftLeft");
  bind("btn-flare", "KeyF");
  bind("btn-takedown", "TouchTakedown");

  const take = document.getElementById("btn-takedown");
  const sync = () => {
    root.dataset.mode = game.status;
    const prompt = game.prompt;
    root.dataset.takedown = prompt ? "1" : "0";
    root.dataset.kind = prompt?.kind || "";
    if (prompt) take.textContent = BUTTON[prompt.kind] || "TAKE DOWN";
    requestAnimationFrame(sync);
  };
  sync();
}

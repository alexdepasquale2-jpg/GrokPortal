export class Input {
  constructor(target = window) {
    this.down = new Set();
    this.pressed = new Set();
    this.stick = { x: 0, y: 0 };
    this.mouse = { x: 0, y: 0, clicked: false };
    target.addEventListener("keydown", (e) => {
      if (e.repeat) return;
      this.down.add(e.code);
      this.pressed.add(e.code);
      if (["Space", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"].includes(e.code)) {
        e.preventDefault();
      }
    });
    target.addEventListener("keyup", (e) => this.down.delete(e.code));
    target.addEventListener("blur", () => {
      this.down.clear();
      this.stick.x = 0;
      this.stick.y = 0;
    });
  }

  tap(code) {
    this.pressed.add(code);
  }

  axis() {
    let x = this.stick.x;
    let y = this.stick.y;
    if (this.down.has("KeyA") || this.down.has("ArrowLeft")) x -= 1;
    if (this.down.has("KeyD") || this.down.has("ArrowRight")) x += 1;
    if (this.down.has("KeyW") || this.down.has("ArrowUp")) y -= 1;
    if (this.down.has("KeyS") || this.down.has("ArrowDown")) y += 1;
    const len = Math.hypot(x, y);
    if (len > 1) {
      x /= len;
      y /= len;
    }
    return { x, y };
  }

  consume(code) {
    if (!this.pressed.has(code)) return false;
    this.pressed.delete(code);
    return true;
  }

  endFrame() {
    this.pressed.clear();
    this.mouse.clicked = false;
  }
}

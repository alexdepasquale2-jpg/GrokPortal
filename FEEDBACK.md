I read the whole spec, not just the opening sections. My short take: I think this is a genuinely strong game concept, and the spec is unusually good at preserving the identity of the game while still giving an implementation agent concrete constraints. The biggest risk isn't that the idea is bad; it's that there are too many good ideas competing to become the first playable game.

I reviewed the full design spec on GitHub

What I really like
1. The core fantasy is distinctive

The best sentence in the document, conceptually, is:

“The swarm is inept Goliath robots ... until a Sancient puppets them.”

That's a game mechanic, narrative premise, and difficulty curve all at once.

The competence/lethality inversion is especially clever:

Strong robots are stupid.
Weak robots can actually aim.
The Sancient doesn't simply spawn a stronger enemy—it temporarily makes the existing enemies competent.
The player therefore learns that “the robots are idiots” is normally safe, and then the game periodically violates that assumption.

That's much more memorable than simply having increasingly powerful enemy tiers.

2. NNN is an excellent central mechanic

The dark → NNN → lit transition may be the strongest systemic idea in the document.

You're effectively making the player choose:

“Do I want the resources and automation badly enough to turn the monsters on?”

That gives the operation a natural escalation curve without needing an artificial “wave 1 / wave 2 / wave 3” structure.

And I particularly like that building itself contributes to loudness. It means the fort isn't just something you construct after the combat—it becomes part of the threat ecosystem.

That has the potential to create some fantastic moments:

“We desperately need this refinery, but turning it on will light the node, which will attract the Sancient, and we're already too loud.”

That's the kind of emergent decision I want this game to produce.

3. The campaign persistence model is very smart

The spec avoids one of the classic problems with hybrid survival/campaign games.

You explicitly separate:

operation state → disposable
campaign state → persistent

That's a good decision.

The particularly good part is:

“Captured production is not deleted. The new owner runs it, scraps it, or trades it.”

That gives the campaign graph memory with consequences without turning the game into an MMO simulator.

It also makes failure interesting.

You don't simply lose:

“I died and lost my run.”

You potentially create:

“I died, and now the Nobots own that refinery.”

That's substantially more compelling.

4. The building system has a real reason to exist

I was initially worried that “Survivors + Rust + RimWorld” could turn into feature soup.

But the dual-resolution building idea helps differentiate it.

Top-down:

“Stamp this fort.”

First-person:

“I'm personally putting this wall exactly here.”

Third-person:

“I'm somewhere in between.”

And the blueprint sandbox means construction isn't merely a UI menu—it can become a creative/preparation activity.

The live ghost system is also excellent because it gives the player something visually legible to work toward.

5. The scope discipline is better than the premise suggests

This is important.

The document sounds enormous, but the v1 section does a surprisingly good job cutting things away.

You explicitly defer:

Pyron Chrome
arming Nobots
multiple biomes
colony simulation
PvP
full diplomacy
warfront snowball
voxel construction
MMO ticking

That's exactly what I wanted to see.

And the implementation order is sensible: pawn → camera → clock → building → blueprint → NNN → Sancient → campaign → meta → Nobots.

Where I think the spec needs work
1. The biggest problem: v1 is still too big

Even though you've cut a lot, your v1 is still essentially asking for:

a complete game loop + three cameras + three combat paradigms + procedural generation + building + blueprint editor + networking + AI + faction behavior + persistent campaign + economy + meta progression + leaderboards.

That's a lot for a first implementation.

The most dangerous line is probably:

“v1 must still feel like: David vs inept Goliaths, radio-on is scary, ghosts want scrap, the field you lose is still someone’s field.”

I agree completely with the goal.

But you don't need every system operational to prove that fantasy.

I'd make a much smaller vertical slice first

Something like:

One cavern.
One storm length: 15 minutes.
One camera: top-down.
One resource: scrap.
One building module.
One weapon.
One robot.
One NNN.
One Sancient.
One extraction.
One persistent site ownership flag.

If that is fun, everything else becomes expansion.

If that isn't fun, the campaign graph, blueprint editor, three-camera system, etc. won't save it.

2. Three cameras may be your biggest unnecessary early complexity

I love the concept.

I don't love implementing it immediately.

The document says:

top-down = auto-fire
third-person = hybrid
first-person = manual

That's effectively three different combat games sharing one character.

And you're also coupling building behavior to the camera.

That's a huge design surface.

I'd strongly consider making the initial vertical slice top-down only, but architecting the pawn so the camera abstraction can be added later.

Then prove:

Is SkyNeet Survivors fun?

before proving:

Is SkyNeet Survivors fun in three fundamentally different control schemes?

The three-camera concept is a fantastic differentiator. I just wouldn't let it become the thing that slows down validation of the actual game.

3. The “flavor” system needs a measurable consequence

I like the philosophy:

“You never pick militant from a menu.”

That's good.

But currently “militant / logistics / diplomatic / subversive / expedition” risks becoming labels attached to player behavior rather than meaningful systems.

For example:

Shooting → militant
Hauling → logistics
Feeding a pocket → diplomatic
Jacking a node → subversive

Cool.

But then what?

I'd define one concrete mechanical consequence for each flavor.

For example:

Militant: combat feats / weapon progression
Logistics: better throughput / cheaper construction
Diplomatic: neutral factions become more useful
Subversive: sabotage / node manipulation
Expedition: exploration / deeper strata

Then the player's behavior gradually changes what opportunities the campaign presents.

Otherwise I'd probably leave flavor tags as telemetry until the underlying systems exist.

4. The economy needs one more layer of definition

“RimWorld-like economy” is a huge phrase.

The actual spec is much more restrained, which is good, but I'd clarify:

What is the fundamental economic decision?

Right now I see:

scrap → construction/production → stockpile → pipeline → campaign ownership

That's enough for v1.

I'd resist adding currencies.

In fact, I'd make scrap the primary physical resource initially.

The more the player has to understand:

scrap + fuel + food + power + ammunition + refined metal + etc.

the more you're moving away from the very clean Survivors loop.

The game's economy should probably feel like logistics under pressure, rather than spreadsheet management.

One thing I'd change conceptually
Make NNN the game's “button”

I think NNN could be even more important than the current spec suggests.

I'd treat it almost like the defining player decision of SkyNeet Survivors:

Before NNN

The Hole

exploration
scavenging
planning
quiet construction
gathering
preparation
After NNN

The War

machines awaken
production becomes possible
loudness increases
Sancient becomes possible
Nobots become interested
fortifications matter
extraction becomes dangerous

That gives every operation an incredibly understandable structure:

Explore → prepare → decide when to wake the hole → survive the consequences → leave or hold.

That's a killer loop.

The Sancient is potentially your signature enemy

I'd lean into this much harder.

The current implementation says:

puppets nearby robots → competence rises → window ends.

Mechanically that's good.

But imagine the player learning the rules well enough that the Sancient becomes something they fear hearing/seeing rather than simply something with a health bar.

For example:

Normal robot behavior:

Giant robot walks into wall.

Player:

“lol.”

Sancient appears.

Same robot:

turns
acquires target
waits
fires

Player:

“Oh shit.”

That is a very strong psychological reversal.

I would preserve that at all costs.

The Nobots are where I would be careful

They're cool, but they're also the faction most likely to turn into “another system because the design says there should be another faction.”

Their strongest current idea is:

They want your NNN.

That's enough.

I wouldn't implement diplomacy, barter, radicalization, Trojan bait, etc. early.

Make Nobots stupidly simple initially:

NNN comes online → Nobots want it → Nobots attack/occupy → whoever controls it owns the production.

That's already enough to create emergent stories.

Later you can make them much more interesting.

The persistent campaign could become the real killer feature

This is the part I'd actually emphasize in marketing/design.

Not:

“It's Vampire Survivors meets Rust meets RimWorld.”

That's useful shorthand, but it's not the actual hook.

The real hook is:

Every run changes the battlefield for the next run.

You go into a hole.

You turn on the node.

You build a refinery.

You barely survive.

You die.

Next time you return:

the refinery is still there—but the Nobots own it.

That's fantastic.

And it gives the game a reason to remember your failures.

One technical concern

The spec is commendably specific about s&box, MCP, host authority, GameResource, FileSystem.Data, NavMeshArea, etc.

That's good for agents.

But I'd be careful about treating specific API implementation decisions as design truth.

For example, statements like:

“three CameraComponents + IsMainCamera”

or

“dynamic NavMeshArea blocker”

are implementation choices.

I'd distinguish:

Design invariant

There is one pawn and three switchable views.

from:

Current implementation assumption

Those views are implemented using three CameraComponents.

That makes the spec more resilient when s&box APIs change or an implementation turns out to be awkward.

The same applies to some of the MCP/editor specifics.

I would add one missing section
“What makes this fun?”

The spec is very good at answering:

What exists?

It's less explicit about:

What should the player feel every 30 seconds?

I'd write down perhaps 5 intended player emotions:

Greed — “There's more scrap over there.”
Dread — “Should we turn on NNN?”
Power — “This fort is becoming ridiculous.”
Panic — “The Sancient made them competent.”
Consequence — “We lost this site, and now somebody else owns it.”

Those are basically the emotional pillars of the game.

If a proposed feature doesn't reinforce one of them, it should probably be questioned.

My overall score

Concept: 9/10
Systems design: 8.5/10
Originality: 9/10
Spec quality: 9/10
v1 scope: 6.5/10
Implementation clarity: 8.5/10
Potential if the core loop works: very high

The thing I like most is that this doesn't feel like a random pile of mechanics. There is a central causal chain underneath it:

Scavenge → build → activate NNN → wake machines → make noise → attract Sancient/Nobots → defend/escape → alter territory → return to a changed world.

That's a real game.

My strongest recommendation would be: don't add anything else yet. Make the smallest possible playable version of that causal chain and see if it's fun. If the 15-minute “wake the hole and survive what you caused” loop works, then earn the cameras, deeper economy, diplomacy, more biomes, and campaign complexity.

And honestly, I think the “field you lose is still someone else's field” idea is the part worth protecting most aggressively. That's the piece that could make SkyNeet Survivors feel like its own game rather than a mashup of its inspirations. 

# Flight Simulator - Pilot Tutorial

Welcome to the **Flight Simulator**! This guide will walk you through the controls, flight dynamics, and tips for flying your aircraft safely from takeoff to landing.

## 🛩️ Flight Controls

The aircraft uses a simplified but realistic flight model. The controls behave differently depending on whether you are on the ground or in the air.

### Speed Control (Throttle)
* **Accelerate:** `W` or `Up Arrow`
* **Decelerate / Brake:** `S` or `Down Arrow`
* *Note:* If you release the throttle, the aircraft will slowly drag to a halt (or glide smoothly when airborne).

### Steering (Yaw & Roll)
* **Turn Left / Right:** `A` / `D` or `Left Arrow` / `Right Arrow`
* **On Duty (Ground):** The aircraft will steer flat on the ground.
* **In Flight (Airborne):** The aircraft will realistically **bank (roll)** up to a maximum angle (default 45°) while turning (yawing). Release the turn keys, and the aircraft will slowly auto-level itself.

### Pitch Control (Nose Up / Down)
* **Nose Up (Climb):** `Down Arrow` (simulates pulling back on the flight stick)
* **Nose Down (Dive):** `Up Arrow` (simulates pushing forward on the flight stick)
* *Note:* Pitching allows you to climb or dive in true 3D space.

---

## 🛫 Takeoff Procedure

1. **Throttle Up:** Hold `W` or `Up Arrow` to build up speed.
2. **Reach Takeoff Speed:** Wait until your speed reaches the minimum takeoff threshold (default is 20 km/h).
3. **Lift Off:** Press and hold **`Space`**. The nose will pitch up, and the aircraft will build lift and ascend into the air. 
4. **Transition to Flight:** Once you are comfortably in the air, release `Space`. You can now use the arrow keys to control pitch and roll manually.

---

## 🛬 Landing Procedure

Landing requires precision and a smooth approach to the ground. 

1. **Approach the Runway:** Align your aircraft with your intended landing zone using the steering controls.
2. **Reduce Speed:** Ease off the throttle or press `S` / `Down Arrow` to reduce your speed, but don't stall!
3. **Initiate Landing Mode:** When you are ready to descend, hold **`Left Shift`**. 
   * Handing control over to the landing sequence will start a smooth descent.
   * The closer you get to the ground, the more the aircraft will naturally auto-level its nose.
4. **Touchdown:** Keep holding `Left Shift`. As altitude reaches ~0, the aircraft will smoothly settle onto the ground, leveling out completely.
5. **Brake:** Once the landing status triggers, use `S` / `Down Arrow` to brake and come to a full stop.

---

## 🛠️ Flight Dynamics Customization

You can adjust how the aircraft handles directly in the Unity Inspector by selecting the object with the `Controls` script attached.

* **Flight Dynamics:**
  * `Max Bank Angle`: How steeply the plane rolls when turning (default: 45°).
  * `Roll Control Speed`: How fast the plane banks into a turn.
  * `Pitch Control Speed`: How responsive the plane is when pulling up or pushing down.

* **Realistic Takeoff Settings:**
  * `Takeoff Speed`: The minimum speed required before lift-off.
  * `Max Climb Angle`: Limits how steeply the nose rises during auto-takeoff.

* **Landing Settings:**
  * `Descent Speed`: Your vertical sink rate when holding `Left Shift`.
  * `Nose Dive Rate`: How much the nose angles down during descent.

## 📊 Heads Up Display (GUI)
When playing, a simple on-screen display in the upper left corner will show:
* **Speed:** Current speed in km/h.
* **Altitude:** Height above the ground level in meters.
* **Status:** Current phase of flight (`GROUND`, `AIRBORNE`, or `LANDING`). 

Enjoy your flight! Feel free to tweak the parameters to find the perfect balance between arcade simplicity and simulator realism.

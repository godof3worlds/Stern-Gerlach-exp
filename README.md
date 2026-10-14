# ⚛️ Stern–Gerlach Experiment Simulator
___
**simulation link** : https://play.unity.com/en/games/022d70af-072a-4554-a4e4-8c1078db65b4/stern-gerlach-experiment-simulator
---
A **3D interactive Stern–Gerlach experiment simulator** built with **Unity 6** and **C#**.

The project visualizes quantum spin measurement by allowing users to configure atoms, place and rotate Stern–Gerlach magnets, observe particle deflection, and analyze the resulting detection patterns.

The simulator combines a **3D physics environment**, interactive controls, and simplified quantum-mechanical measurement logic to make the Stern–Gerlach experiment easier to understand visually.

---

## ✨ Features

- 🧲 Interactive Stern–Gerlach magnets
- ⚛️ Configurable atomic number
- 🎯 Multiple sequential Stern–Gerlach analyzers
- 🔄 Adjustable magnet position and rotation
- 🛰️ 3D atom beam simulation
- 📐 Spin-dependent particle deflection
- 🖥️ Detector screen for observing measurement results
- 🎨 Interactive Unity UI Toolkit interface
- 🔬 Support for different angular-momentum states
- 📊 Visual observation of quantized beam splitting
- 🚀 Real-time 3D visualization

---

## 🧪 What Does the Simulator Demonstrate?

The Stern–Gerlach experiment demonstrates that angular momentum is **quantized**.

Instead of producing a continuous distribution when passing through an inhomogeneous magnetic field, atoms are detected in discrete paths corresponding to allowed projections of angular momentum.

For a system with total angular momentum \(J\), the allowed magnetic quantum-number states are:

\[
m_J=-J,-J+1,\ldots,J-1,J
\]

giving:

\[
\boxed{2J+1}
\]

possible states.

For example:

| System | \(J\) | Possible states | Ideal number of paths |
|---|---:|---:|---:|
| Spin-½ | ½ | −½, +½ | 2 |
| Nitrogen model | 3/2 | −3/2, −½, +½, +3/2 | 4 |
| \(J=2\) | 2 | −2, −1, 0, +1, +2 | 5 |

---

# 🏗️ Project Architecture

The simulator is primarily controlled by three core scripts:

```text
                    ┌─────────────────────┐
                    │   Unity UI Toolkit  │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │   MagnetManager     │
                    │                     │
                    │ • Magnet settings   │
                    │ • Atom settings     │
                    │ • Detector position  │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │    AtomSpawner      │
                    │                     │
                    │ Creates atom beam   │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │      SIlverIO       │
                    │                     │
                    │ • Quantum state     │
                    │ • Measurement       │
                    │ • Deflection        │
                    │ • Detection         │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │    Detector Screen  │
                    │                     │
                    │   Final spot        │
                    └─────────────────────┘
```

---

# 📁 Main Scripts

## `SIlverIO.cs`

The main atom/particle controller.

It handles:

- Atom initialization
- Atomic-number configuration
- Total angular momentum \(J\)
- Spin-state selection
- Magnetic-field detection
- Quantum measurement
- Particle deflection
- Detector collision
- Particle cleanup

The atom disables gravity and travels initially along its forward direction.

```csharp
rb.useGravity = false;
rb.linearVelocity = transform.forward * speed;
```

---

## `AtomSpawner.cs`

Responsible for generating the particle beam.

Atoms are periodically instantiated from the atom prefab.

A small random positional offset is introduced so the beam has a finite width rather than all particles originating from exactly the same point.

---

## `MagnetManager.cs`

Controls the experiment configuration and UI interaction.

It manages:

- Creating magnets
- Selecting magnets
- Magnet position
- Magnet rotation
- Atomic number
- Particle speed
- Particle lifetime
- Detector position

The UI is implemented using **Unity UI Toolkit**.

---

# ⚙️ Simulation Flow

The complete simulation can be represented as:

```text
        START
          │
          ▼
   Configure Experiment
          │
          ├── Atomic Number
          ├── Particle Speed
          ├── Magnet Position
          ├── Magnet Rotation
          └── Detector Position
          │
          ▼
     Spawn Atom
          │
          ▼
   Determine Angular
      Momentum J
          │
          ▼
   Determine allowed
       mJ states
          │
          ▼
      Atom moves
      toward magnet
          │
          ▼
   ┌──────────────────┐
   │ Magnetic Field?  │
   └────────┬─────────┘
            │ YES
            ▼
     Measure Spin
            │
            ▼
      Calculate
       Deflection
            │
            ▼
      Apply Force
            │
            ▼
    Continue to next
        analyzer
            │
            ▼
    ┌─────────────────┐
    │ Detector Screen │
    └────────┬────────┘
             │
             ▼
        Stop Atom
             │
             ▼
       Visible Spot
```

---

# 🧲 Magnetic Field Detection

When an atom enters a collider tagged as `Field`, the simulator identifies the magnet's measurement axis using:

```csharp
Vector3 fieldAxis = other.transform.up.normalized;
```

The particle then performs two operations:

```csharp
MeasureSpin(fieldAxis);
ApplyDeflection(fieldAxis);
```

Therefore, rotating the magnet changes the axis along which the particle is measured.

---

# 🌀 Quantum State Selection

The simulator determines the number of allowed states from:

\[
N=2J+1
\]

The code uses:

```csharp
numStates = Mathf.RoundToInt(2f * totalJ + 1f);
```

The initial state is selected from the allowed \(m_J\) values.

For example, when:

\[
J=\frac{3}{2}
\]

the possible values are:

\[
m_J=
-\frac32,
-\frac12,
+\frac12,
+\frac32
\]

giving four possible states.

---

# ⚡ Particle Deflection

The implemented deflection is based on the normalized spin projection:


\vec{F}_{\mathrm{sim}} \propto \frac{m_J}{J}\,\vec{B}_{\mathrm{axis}}


The simulator then applies a deflection along the magnetic-field axis.

Conceptually:

\[
\vec F_{\text{sim}}
\propto
\frac{m_J}{J}
\vec B_{\text{axis}}
\]

This produces different trajectories for different spin projections.

Extreme states produce the largest deflection, while a state with:

\[
m_J=0
\]

produces no deflection in this model.

---

# 🔬 Sequential Stern–Gerlach Experiment

The simulator supports placing multiple analyzers sequentially.

For example:

```text
Atom Beam
   │
   ▼
┌───────────┐
│ Magnet 1  │
└─────┬─────┘
      │
      │ Spin Measurement
      ▼
   Split Beam
      │
      ▼
┌───────────┐
│ Magnet 2  │
└─────┬─────┘
      │
      │ Measurement
      │ along another axis
      ▼
   New Splitting
      │
      ▼
 Detector Screen
```

This allows the effect of changing the measurement axis to be visualized.

---

# 📐 Quantum Mechanics Reference

For the ideal **spin-½** case, the probability of obtaining the same spin state when the second analyzer is rotated by an angle \(\theta\) is:

\[
\boxed{
P(+|+)=\cos^2\left(\frac{\theta}{2}\right)
}
\]

and the opposite result is:

\[
\boxed{
P(-|+)=\sin^2\left(\frac{\theta}{2}\right)
}
\]

These probabilities arise from the quantum-mechanical overlap of spin states and the Born rule.

### Example

| Angle \(\theta\) | Same result | Opposite result |
|---:|---:|---:|
| 0° | 100% | 0% |
| 45° | 85.36% | 14.64% |
| 90° | 50% | 50% |
| 135° | 14.64% | 85.36% |
| 180° | 0% | 100% |

---

# 💻 Code vs Quantum Mechanics

For a spin-½ system, the simulator contains the following probability logic:

```csharp
float angleRad = angleDeg * Mathf.Deg2Rad;

float probUp =
    Mathf.Pow(Mathf.Cos(angleRad / 2.0f), 2.0f);

spin = (Random.value < probUp)
       ? 0.5f
       : -0.5f;
```

This corresponds to:

\[
P(+)=\cos^2\left(\frac{\theta}{2}\right)
\]

and therefore correctly reproduces the ideal spin-½ angle dependence.

### Important

This formula applies specifically to **spin-½ systems**.

It should not be directly generalized to arbitrary \(J\).

---

# 🧮 Higher Angular Momentum States

For systems with:

\[
J>\frac12
\]

the full quantum-mechanical transition probability is described using the **Wigner small-d matrix**:

\[
\boxed{
P(m'|m,\theta)
=
|d^j_{m'm}(\theta)|^2
}
\]

The current simulator uses a simplified state-selection approach for these higher-\(J\) cases rather than implementing the complete Wigner d-matrix calculation.

This distinction is important when comparing the simulator with a full quantum-mechanical model.

---

# 🖥️ Detector

When an atom reaches a collider tagged `Screen`, the simulator stops the atom.

The particle:

1. Stops moving
2. Stops rotating
3. Becomes kinematic
4. Becomes attached to the detector
5. Remains visible as a detection point

Over many particles, these points create the characteristic beam-splitting pattern.

---

# 🎮 User Interaction

The simulator allows the user to interact with the experiment rather than simply watching a predefined animation.

The user can configure:

- Atomic number
- Particle speed
- Particle lifetime
- Magnet position
- Magnet rotation
- Number of magnets
- Detector position

This makes it possible to investigate how changing the experimental setup affects the observed trajectories.

---

# 🧰 Technologies Used

| Technology | Purpose |
|---|---|
| **Unity 6** | 3D simulation environment |
| **C#** | Simulation and interaction logic |
| **Unity Physics** | Particle movement and collision detection |
| **UI Toolkit** | Interactive user interface |
| **UXML** | UI structure |
| **USS** | UI styling |
| **Rigidbody** | Particle motion |
| **Colliders / Triggers** | Magnetic field and detector detection |

---

# 📂 Project Structure

```text
Stern-Gerlach-exp/
│
├── Assets/
│   ├── Script/
│   │   ├── SIlverIO.cs
│   │   ├── MagnetManager.cs
│   │   ├── AtomSpawner.cs
│   │   ├── SilverSc.cs
│   │   ├── PlayerMovement.cs
│   │   └── SternGerlachHUDController.cs
│   │
│   ├── Scenes/
│   │   ├── HomeMenue.unity
│   │   ├── MainScene.unity
│   │   └── SampleScene.unity
│   │
│   └── UI/
│       ├── SternGerlachUI.uxml
│       └── SternGerlachHUD.uxml
│
├── ProjectSettings/
│
└── README.md
```

---

# 🚀 Running the Project

### Requirements

- Unity Hub
- Unity **6000.5.0f1** or a compatible Unity 6 version
- Windows / compatible Unity platform

### Steps

1. Clone the repository.

```bash
git clone https://github.com/godof3worlds/Stern-Gerlach-exp.git
```

2. Open **Unity Hub**.

3. Select **Add Project from Disk**.

4. Select the cloned project folder.

5. Open the project using the appropriate Unity version.

6. Open the main simulation scene.

7. Press **Play**.

---

# 🧪 Suggested Experiment

A simple way to explore the simulator:

### Experiment 1 — Spin-½ Beam Splitting

1. Set the atom to **Silver (Ag)**.
2. Place one Stern–Gerlach magnet.
3. Place the detector after the magnet.
4. Run the simulation.
5. Observe the two main detection regions.

### Experiment 2 — Sequential Analyzers

1. Use Silver.
2. Add a second magnet.
3. Rotate the second magnet relative to the first.
4. Run multiple particles.
5. Compare the resulting distribution.

### Experiment 3 — Higher Angular Momentum

1. Select an element represented by a higher \(J\).
2. Pass the particles through the analyzer.
3. Observe the increased number of possible trajectories.

---

# ⚠️ Physics Model

This project is an **educational simulation**, not a complete numerical simulation of the electromagnetic Stern–Gerlach experiment.

The current implementation simplifies several physical processes.

In particular:

- Magnetic deflection is implemented as a simulation force/kick rather than calculating a complete magnetic-field gradient.
- The spin-½ probability model uses the \(\cos^2(\theta/2)\) relationship.
- Higher-\(J\) systems use a simplified state-selection model rather than a full Wigner d-matrix implementation.
- Atomic configurations are represented through the project's atomic-number-to-\(J\) mapping rather than a complete atomic-structure calculation.

The purpose is to provide an **interactive visual representation of quantum measurement and spin quantization**.

---

# 🔮 Future Improvements

Possible extensions include:

- [ ] Implement the full Wigner d-matrix transition probabilities
- [ ] Add physically calculated magnetic-field gradients
- [ ] Replace instantaneous deflection with continuous force integration
- [ ] Add probability/distribution graphs
- [ ] Display measured \(m_J\) values in real time
- [ ] Add experimental data comparison
- [ ] Add Bloch-sphere visualization for spin-½ systems
- [ ] Add measurement statistics
- [ ] Export experimental results
- [ ] Improve atomic-structure modelling
- [ ] Add more detailed magnetic-field visualization

---

# 🎓 Educational Purpose

This project was developed to provide a visual and interactive way of studying:

- Quantum measurement
- Spin
- Angular momentum quantization
- Magnetic quantum numbers
- Stern–Gerlach analyzers
- Sequential quantum measurements
- Probability amplitudes
- Born-rule probabilities
- Spin-½ systems
- Higher angular-momentum systems

---

# 👨‍💻 Project

**Stern–Gerlach Experiment Simulator**

Built using **Unity + C#**

Repository:

**https://github.com/godof3worlds/Stern-Gerlach-exp/**

---

## 📜 License

MIT license 

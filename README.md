# PathGrid: 2D AI Pathfinding & Traffic System

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=for-the-badge&logo=c-sharp)
![Algorithms](https://img.shields.io/badge/Algorithms-A*_%7C_BFS_%7C_Greedy-success?style=for-the-badge)

An interactive, grid-based 2D traffic and pathfinding simulation built with Unity. This project demonstrates how autonomous agents navigate through dynamic urban environments using different search algorithms while respecting real-time traffic rules.

## Play the WebGL Demo

Experience the simulation directly in your browser:  
👉 **[Play PathGrid Live!]()**

---

## Sneak Peek

![Simulation Gameplay]()  
*Dynamic path recalculation and algorithm switching in real-time.*

---

## Key Features

* **Real-Time Algorithm Switching:** Seamlessly switch between three fundamental pathfinding algorithms to compare their efficiency and computational cost:
    * **A* (A-Star):** The optimal approach balancing `G-Cost` (distance from start) and `H-Cost` (heuristic distance to target).
    * **Greedy Best-First Search:** A faster, heuristic-only approach that moves strictly towards the target.
    * **BFS (Breadth-First Search):** Explores equally in all directions, guaranteeing the shortest path in unweighted grids.
* **Dynamic Traffic Penalty:** Traffic lights dynamically alter the movement cost of nodes in real-time. The AI recalculates its path or waits based on the current state of the intersection.
* **Interactive Node Debugger:** Hover over any grid cell to instantly inspect its walkability, grid coordinates, and active algorithm metrics (G, H, and F costs).
* **Visual Overlays:** Clean UI with global pause darkening effects and clear LineRenderer path visualizations.

---

## Technical Architecture

The project is built with strict adherence to **Object-Oriented Programming (OOP)** principles and **Separation of Concerns**:

* **Decoupled UI & Logic:** The `UIManager` handles all canvas elements, buttons, and visual states independently, while the `GameManager` strictly controls the simulation state, time scale, and entity lifecycle.
* **Modular AI (`CarBrain`):** The vehicle logic is abstracted to allow easy implementation of new search algorithms without altering the core movement script (`CarMotor`).
* **Optimized Grid Management:** A 2D grid system that calculates node neighbors and distances efficiently, utilizing LayerMasks for precise raycasting and obstacle detection.

---

## Dev Notes & Future Roadmap

While the core architecture strongly adheres to OOP and Separation of Concerns, a few "quick and dirty" rapid-prototyping solutions were used to deliver the playable WebGL demo faster. These areas are logged as technical debt and are slated for refactoring in the upcoming updates.

**Planned Features for Next Releases:**
* **Custom Map Editor:** Giving users the freedom to build their own simulation environments by placing walls, roads, and traffic lights dynamically.
* **Algorithm Visualizer (Maze Mode):** A dedicated mode that animates the step-by-step node scanning process (wave expansion effect), rather than just drawing the final path.
* **3D Port:** Initial planning and architectural design to bring the simulation into a fully 3D environment.
* **General Polish:** Visual updates, UI/UX enhancements, and minor bug squashing.
* This README and aspects of the development workflow were optimized with the assistance of Google Gemini.*

## Controls
Left Click: Place the starting point (Car spawn).
Right Click: Place the target destination.
UI Panel: Start/Pause simulation, toggle path visibility, and switch active algorithms.


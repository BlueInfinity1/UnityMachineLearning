# Driver Agent: ML-Driven Race Track Navigation

This repository implements a **Driver Agent** using Unity's **ML-Agents** framework. The agent is trained to navigate a race track efficiently by passing through checkpoints in the correct order, staying on the inside of the track, and avoiding collisions. The goal is to create a driver that learns optimal racing behavior over time.

[Watch a video demo of the Driver Agent in action](https://drive.google.com/file/d/1Kc6kzMUXOT30NgO9EWadSUQ2ZywEyHss/view?usp=sharing)

---

## Overview

The **Driver Agent** is a Unity ML-Agents-based AI agent that learns to drive on a race track by maximizing rewards and minimizing penalties through reinforcement learning.

### Key Features

- **Checkpoint System**: The agent learns to pass through indexed checkpoints on the race track in the correct order.
- **Speed Optimization**: Faster checkpoint completion results in higher rewards, incentivizing the agent to drive efficiently.
- **Inside Track Preference**: The agent is encouraged to stay on the inner part of the track by rewarding proximity to "Inside Track Guide" objects.
- **Collision Avoidance**: Colliding with track walls results in penalties, teaching the agent to avoid walls.

---

## Training Behavior

The training process focuses on optimizing the following behaviors:
1. **Checkpoint Navigation**: Passing through the next checkpoint in sequence gives positive rewards.
2. **Speed Encouragement**: Rewards are scaled based on how quickly the agent reaches the next checkpoint.
3. **Inside Track Bonus**: Rewards for staying close to "Inside Track Guide" objects.
4. **Collision Penalty**: Penalties for hitting or staying in contact with track walls.

---

## Input System

- **Discrete Actions**: The agent uses discrete inputs to control acceleration (`0 or 1`) and turning (`-1, 0, or 1` for left, none, and right).
- **Heuristic Mode**: Manual controls for testing allow human players to drive using the keyboard (`Space` for acceleration, `A/D` for turning).


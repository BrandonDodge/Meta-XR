# **Meta-XR**

**Modular XR Dashboard Platform for Meta Quest Pro**

Meta-XR is a modular extended reality (XR) dashboard platform designed for the **Meta Quest Pro**. The system enables dynamic, slot-based widget modules (weather, health metrics, telemetry, and more) to render as a heads-up display (HUD) inside immersive environments.

The architecture cleanly separates:

* Unity XR Interface Layer (headset rendering and spatial UI)

* Android Companion Layer (sensor access and permissions)

* Modular Widget SDK (pluggable dashboard components)

* Structured Data Bridge (Android ↔ Unity communication)

The long-term objective is to build a scalable, privacy-aware XR interface that supports independently developed widgets while maintaining strict performance and lifecycle constraints.

# **Repository Structure**

Meta-XR/  
│  
├── Unity/  
├── android/  
└── Documents/

# **Unity/**

Contains the Unity XR project targeting **Meta Quest Pro**.

## **Responsibilities**

* World-space HUD rendering

* Slot-based layout manager

* Widget lifecycle management

* Event bus for inter-widget communication

* Performance budgeting and isolation enforcement

* Android bridge integration

## **Key Components**

* XR Rig & Camera configuration

* Dashboard layout manager

* Widget SDK contract definitions

* Unity ↔ Android communication layer

* Prefab templates for widget modules

## **Requirements**

* Unity (LTS recommended)

* Meta XR SDK / Oculus Integration

* Android Build Support Module

# **android/**

Android Studio project serving as the companion application layer.

## **Responsibilities**

* Sensor data acquisition (health, GPS, motion, etc.)

* Permission & consent management

* Data serialization and versioning

* Local data handling (privacy-first model)

* Communication bridge to Unity application

## **Architectural Goals**

* Modular project structure

* Explicit runtime permission framework

* Revocable data consent handling

* Local-only data retention by default

* Structured, versioned data models

## **Build Requirements**

* Android Studio (latest stable)

* Gradle (auto-managed)

* Target SDK aligned with Meta Quest compatibility

# **Documents/**

Project documentation, design artifacts, and sprint deliverables.

## **Contents May Include**

* Sprint artifact documentation

* Architecture diagrams

* Data flow specifications

* Widget SDK contracts

* Performance constraints

* Security and privacy design notes

* Research findings and technical evaluations

This folder functions as the formal design and traceability repository for development iterations.

# **Architecture Overview**

## **High-Level Data Flow**

Android Sensors  
     ↓  
Permission Framework  
     ↓  
Data Serialization Layer  
     ↓  
Bridge Interface  
     ↓  
Unity XR Runtime  
     ↓  
Widget Rendering Layer

## **Design Principles**

* Modular widget isolation

* Explicit lifecycle management

* Publish/subscribe event model

* Performance budgeting per widget

* Local-first data handling

* Clear interface contracts

# **Widget Development Model**

Widgets are independently developed modules that adhere to a defined interface contract.

## **Widget Requirements**

* Defined lifecycle methods (initialize, update, destroy)

* Size and layout constraints

* Performance budget adherence

* No direct coupling to other widgets

* Communication only through global event bus

* Structured data input model compliance

This enables parallel development while preserving architectural integrity.

# **Current Status**

* Core XR HUD baseline implemented

* Repository restructured with Unity project isolated under `/Unity`

* Android companion skeleton in progress

* Widget SDK contract formalization underway

* Dashboard layout enhancements planned

# **Future Roadmap**

* Expand widget SDK documentation

* Implement global event bus

* Add health metrics widget

* Add weather widget

* Optimize world-space layout scaling

* Integrate secure Android-to-Unity IPC

* Formalize performance benchmarking framework

# **Development Setup**

## **Unity**

1. Open `/Unity` project in Unity Hub

2. Ensure Meta XR SDK is installed

3. Set Build Target to Android

4. Deploy to Meta Quest device

## **Android**

1. Open `/android` in Android Studio

2. Sync Gradle

3. Configure device or emulator

4. Build and test companion app

# **Privacy & Security Model**

* All data is local by default

* Explicit runtime permissions required

* User consent framework is revocable

* No external transmission unless explicitly enabled

* Structured versioned models prevent schema ambiguity

# **Contributing**

This project follows a structured sprint model with documented artifacts under `/Documents`.

When contributing:

* Follow widget SDK contract definitions

* Maintain modular isolation

* Respect performance budgets

* Document architectural changes
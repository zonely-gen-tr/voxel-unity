# Zonely Voxel Game Engine

Zonely tarafından geliştirilen Unity 6 tabanlı, WebGL odaklı voxel oyun motoru ve geliştirici altyapısı.

> Bu repository motor geliştirme, runtime sistemleri, WebGL build otomasyonu ve geliştirici araçları içindir. Oyun/proje özelinde tanıtım veya son kullanıcı dokümantasyonu tutulmaz.

## Engine Structure

```text
Assets/
  Scripts/        Runtime engine systems
  Editor/         Build/editor tooling
Packages/         Unity package dependencies
ProjectSettings/  Unity version/project settings
.github/          CI / WebGL build workflow
tools/            Post-build and single-file tooling
```

## Core Development Areas

- Runtime entity / character foundation
- Third-person controller and camera layer
- Remote asset registry and runtime GLTF/GLB loading
- Equipment / attachment foundation
- Runtime catalog and preview tooling
- WebGL build pipeline
- Single-file WebAssembly packaging pipeline
- Remote data/CDN friendly architecture

## Build

CI workflow:

```text
.github/workflows/webgl-build.yml
```

The workflow builds the Unity project for WebGL and then packages the generated Unity Web output into:

```text
build/Single/zonely-voxel-engine.html
```

Required GitHub Actions secrets are documented in `DEVELOPMENT.md`.

## Version Pins

- Unity: `6000.0.32f1`
- glTFast: `6.9.1`

Versions are intentionally pinned for reproducible WebGL builds.

## Repository Policy

- Keep engine code generic.
- Do not add game-specific marketing copy to this repository.
- Content definitions should move toward remote registry/data files where possible.
- Engine code changes require a WebGL rebuild; remote content/data changes should avoid rebuilds where possible.

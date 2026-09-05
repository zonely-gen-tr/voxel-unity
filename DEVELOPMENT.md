# Development Setup

## GitHub Actions Secrets

Repository -> Settings -> Secrets and variables -> Actions:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For applicable paid Unity licenses, also configure:

- `UNITY_SERIAL`

Without a Unity license secret, the workflow intentionally skips the build step rather than failing the initial repository setup.

## Build Trigger

A push to `main` affecting the engine/project paths triggers the WebGL pipeline. The workflow can also be started manually with `workflow_dispatch`.

## Output Artifacts

- `zonely-voxel-engine-single-html`
- `zonely-voxel-engine-webgl`

The first artifact contains the single HTML WebAssembly build.

## Update Model

Normal engine updates should replace only changed files. Re-uploading the whole project is not required.

Runtime content should increasingly be sourced from remote registries/CDNs so content changes can be deployed independently from engine builds.

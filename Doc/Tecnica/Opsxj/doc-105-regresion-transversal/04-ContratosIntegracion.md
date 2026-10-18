# DOC-105 — Contratos e integraciones

- Ticket: DOC-105
- Cambio OpenSpec: doc-105-regresion-transversal
- Clasificacion: cross_cutting

Cambio: `doc-105-regresion-transversal`. Impacto: `cross_cutting`.

## Contratos e integraciones

El archivo con salvedades no sincroniza especificaciones ni acredita la matriz completa. Los perfiles runtime, archivos documentales y configuración privada quedan fuera del PR. El snapshot focal distingue preview del principal; bytes de recursos completados no equivalen a unión de rangos en vuelo ni exclusivamente a tráfico de red.

### Perfil

`tools/e2e/profiles/doc105-pdf-cross-module.profile.example.json` admite exclusivamente configuración no secreta: ambiente base, módulo, navegador, cuatro redes, caché, cinco consumidores y guardado opcional. Usuario, contraseña y cuenta ODBC se capturan por TTY.

## Transporte

- `RANGE`: HTTP `206`, `Content-Range` coherente y longitud total única.
- `STREAM`: HTTP `200`; nunca se etiqueta como parcial.
- Evidencia: códigos, conteos, tiempos, modo y bytes agregados. Se excluyen URL, query, rutas, IDs, títulos, payloads, cookies, tokens y conexiones.

## Integración del grafo

Cuando `mutation.enabled=true`, el perfil referencia un perfil runtime DOC-103 dentro de `tools/e2e/profiles`. El runner solicita autorización separada y delega en `runReal`, que valida el único POST y los controles `SELECT`.

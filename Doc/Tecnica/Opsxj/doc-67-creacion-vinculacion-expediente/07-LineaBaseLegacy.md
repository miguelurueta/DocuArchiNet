# DOC-67 — Línea base legacy protegida

La migración es aditiva. Esta entrega no modifica ni redirige las funciones o consumidores legacy. La prueba `Tests/importar-servicio-web-legacy-surface-invariance.test.cjs` normaliza finales de línea y valida las siguientes huellas:

| Superficie | Algoritmo | Huella |
|---|---|---|
| `workflow/ClassAlmacenamiento.vb` | Git blob SHA-1 | `b875d24f0a9ff63f24a4fff96f637cb04afb1405` |
| `Gestion/ClassGaExpediente.vb` | Git blob SHA-1 | `2998523902ec2d455ac4b96a644297674d6d14b9` |
| `webservice/WebServiceGaExpediente.asmx.vb` | Git blob SHA-1 | `11a60af88f9b3591e70ca3e91567d34fe39fce7f` |
| `webservice/WebService_integracion_sii.asmx.vb` | Git blob SHA-1 | `580c3832343205f2246aa0acbfcc8f703f2a0ebe` |
| Árbol completo `Integracionccv/` | SHA-256 de ruta y contenido normalizado | `c64fde736b91acef8b3baec5d9bb63d32b39a318a6ac08afae2aa01c6bc45823` |

La misma prueba recorre `DTOs`, `Modelo`, `Services` e `Infrastructure` para impedir llamadas HTTP internas a los ASMX legacy. La invariancia específica de `AlmacenaDocumentoTareaWorkflow` continúa cubierta por `Tests/importar-servicio-web-storage-invariance.test.cjs`.

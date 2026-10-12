# Matriz GO/NO-GO

| Puerta | Evidencia saneada | GO | NO-GO / acción |
|---|---|---|---|
| Paquete | versión y hash del artefacto | Coincide con lo aprobado | Detener despliegue |
| Proyecto | inventario `.vbproj` | Cada fuente/recurso 2FA aparece una vez | Corregir paquete, no desplegar |
| Esquema | conteos de `information_schema` | Tabla, diez columnas e índices requeridos | No ejecutar DDL desde DOC-98; remitir a compatibilidad existente |
| Sesión | comprobación local sin contenido | InProc, 10 minutos | Corregir configuración antes del piloto |
| HMAC | estado booleano por llave | Activa y material disponible, Base64 >=32 bytes | Instalar llave sin mostrarla |
| SMTP | conteo/estado agregado | Una fila `ESTADO_ENVIO=1`, resolución válida | Corregir configuración existente |
| Catálogo | conteo por manifiesto | Una fila activa por empresa/módulo | Resolver ausencia o ambigüedad |
| Principales | conteos agregados por repositorio | Informativo; permite dimensionar remediación | No crea excepciones ni cambia activación transversal |
| Legacy OFF | resultado smoke | Los cuatro tipos conservan login | Revertir paquete/configuración |
| Piloto SQL | candidatos/afectadas/postcondición | 1/1/1 | Rollback exacto; investigar concurrencia |
| Piloto E2E | marcadores saneados | Flujo completo aprobado | Desactivar piloto |
| Oleada | manifiesto/afectadas/postcondición | N/N/N y cero extras | Revertir la oleada, detener siguientes |
| Final | filas activas y smoke por tipo | Todo el manifiesto activo transversalmente | Estado `BLOQUEADO`, no declarar cierre |

## Datos de principal

No hay estado de habilitación 2FA por usuario. Si un principal no puede resolverse de forma única o carece de correo válido, el intento falla cerrado conforme a la implementación. El diagnóstico agregado ayuda a corregir calidad de datos, pero no decide quién recibe 2FA.

## Umbrales operativos

Los umbrales de entrega, finalización y latencia pertenecen a la autorización de cada ambiente. DOC-98 no inventa valores universales. Cualquier umbral ausente antes del piloto es NO-GO operativo.

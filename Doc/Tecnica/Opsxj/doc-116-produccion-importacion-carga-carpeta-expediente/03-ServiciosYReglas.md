# DOC-116 — 03-ServiciosYReglas

- Ticket: DOC-116
- Cambio OpenSpec: doc-116-produccion-importacion-carga-carpeta-expediente
- Clasificacion: cross_cutting

## Servicios y reglas

CrearExpediente consulta histórico antes de reservar y reautoriza destino; no recrea por nombre o antigüedad. Se conserva el adaptador legacy y la captura/checkpoint de IDs existentes. La preparación compartida usa un plan, una confirmación, dos recepciones autorizadas y controles SELECT antes de invocar únicamente CREAR_EXPEDIENTE.

EXP01 tuvo dos creaciones y sus repeticiones; EXP02 cliente descartó el resultado confirmado, consultó checkpoint/identidad y repitió la misma clave/revisión. EXP03 retiró agregar_expediente al actor no propietario, exigió 403 y verificó cero nuevas relaciones; Finally restauró y comprobó por conexión independiente.

Las respuestas inciertas bloquean nuevos efectos. Los IDs conocidos se registran antes de otras verificaciones. No se ejecuta runRec completo ni almacenamiento/materialización documental. Las ventanas internas EXP02 se ejecutaron con una candidata de laboratorio autorizada, sesión/autoridad HTTP real y repositorio/legacy reales. La excepción al devolver el checkpoint confirmado y el fallo previo a checkpoint tienen evidencia propia; los dobles locales conservan su alcance distinto. El runtime normal quedó restaurado.

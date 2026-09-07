# Estados y errores

La intención y sus elementos nacen en `Creada`. `INTENT_IN_PROGRESS` es reintentable; `IDEMPOTENCY_CONFLICT` indica misma clave con contexto distinto; `INVALID_INTENT` e `INTENT_UNAVAILABLE` fallan cerrado.

DOC-52 no ejecuta fases documentales y no promete atomicidad distribuida.

# Flujo de integración

- Ticket: DOC-52
- Cambio OpenSpec: doc-52-extender-contratos
- Clasificacion: cross_cutting

1. Validar contexto y todos los elementos.
2. Construir requisitos/comandos sin inferir del primer elemento.
3. Canonicalizar y calcular SHA-256.
4. Adquirir guard cooperativo.
5. Reservar clave única y persistir agregado.
6. Reutilizar si la huella coincide; retornar `IDEMPOTENCY_CONFLICT` si difiere.

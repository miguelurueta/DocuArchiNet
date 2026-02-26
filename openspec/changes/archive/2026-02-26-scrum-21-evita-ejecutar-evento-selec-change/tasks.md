## 1. Control de disparo de eventos y consultas

- [x] 1.1 Identificar en `RadicacionForm` y hooks asociados los puntos donde un `change` inicial dispara consultas sin interacción del usuario.
- [x] 1.2 Ajustar condiciones `enabled/shouldQuery` para que solo permitan consultas con entrada valida iniciada por el usuario.
- [x] 1.3 Asegurar que valores iniciales (`null`, vacio, placeholders) no activen consultas dependientes de `change`.

## 2. Normalización y estabilidad de estado

- [ ] 2.1 Estandarizar normalizacion de valores antes de construir payloads de consulta para evitar cambios transitorios.
- [ ] 2.2 Garantizar que al limpiar entrada se cancelen consultas activas y se limpie estado derivado de opciones.
- [x] 2.3 Verificar que `Descripcion_Documento` actualice `selectedTramiteId` solo tras seleccion valida del usuario.

## 3. Pruebas y no-regresión

- [x] 3.1 Agregar/actualizar pruebas en `RadicacionForm.spec.test.tsx` con casos de no-disparo en primer render (`[SPEC:...]`).
- [ ] 3.2 Agregar/actualizar pruebas en hooks de autocompletado/restricción para validar gating de consultas por interacción.
- [x] 3.3 Ejecutar suite de pruebas de Radicación y corregir regresiones antes de cerrar el cambio.

## 4. Validación final y evidencia

- [ ] 4.1 Validar manualmente en UI que no se ejecuten APIs por `change` en primer render.
- [ ] 4.2 Validar manualmente que consultas sí se ejecutan al digitar/seleccionar y que la UX se mantiene estable.
- [ ] 4.3 Documentar evidencia de pruebas en el change y preparar archivado.

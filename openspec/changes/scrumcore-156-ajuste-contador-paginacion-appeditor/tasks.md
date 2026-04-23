## 1. Diagnostico y delimitacion del contador

- [x] 1.1 Auditar `presentation/AppEditor.tsx` para identificar donde se monta
  el contador y que dependencias visuales conserva en modo paginado
- [x] 1.2 Auditar `application/usePageContext.ts` para identificar la fuente
  exacta de inestabilidad del calculo de pagina actual
- [x] 1.3 Auditar `application/usePaginationMetrics.ts` para validar si las
  boundaries visuales entregadas al contador son coherentes y suficientes

## 2. Ajuste del modelo de page context

- [x] 2.1 Redefinir el calculo de pagina actual para basarlo en un modelo
  geometrico estable entre `canvas`, `sheet`, `scrollTop` y `zoomLevel`
- [x] 2.2 Reducir el acoplamiento del contador con eventos internos del motor
  de repaginacion que no sean estrictamente necesarios
- [x] 2.3 Confirmar que `pagination-updated` y `scroll` sean las unicas fuentes
  principales de sincronizacion del contador, salvo justificacion tecnica

## 3. Integracion visual del contador

- [x] 3.1 Ajustar la presentacion del contador para que permanezca compacta,
  legible y no invasiva en `paginationMode="visual"`
- [x] 3.2 Confirmar que el contador no interfiera con escritura, foco,
  seleccion o interaccion del editor
- [x] 3.3 Confirmar que el contador siga alineado con hojas visuales y zoom

## 4. Compatibilidad y no regresion

- [x] 4.1 Confirmar que `paginationMode="none"` no herede logica paginada
  innecesaria del contador
- [x] 4.2 Confirmar que el ajuste no rompa serializacion HTML ni inserte
  metadata espuria asociada solo al page context
- [x] 4.3 Confirmar que imagenes, listas, paste largo y continuidad de
  escritura no sufran regresiones por el ajuste del contador

## 5. Pruebas

- [x] 5.1 Agregar o ajustar pruebas de `usePageContext` para cubrir scroll
  estable, boundaries coherentes y sincronizacion inmediata con cambios de
  layout
- [x] 5.2 Agregar o ajustar pruebas de `AppEditor` para validar que el contador
  refleje la hoja activa en modo visual
- [x] 5.3 Agregar o ajustar pruebas de compatibilidad con zoom visual
- [x] 5.4 Agregar o ajustar pruebas para asegurar ausencia de regresion en modo
  continuo
- [x] 5.5 Ejecutar la suite focalizada de `AppEditor` y registrar evidencia

## 6. Validacion final

- [x] 6.1 Documentar que cambio en el calculo del contador y por que el nuevo
  enfoque es mas estable
- [x] 6.2 Documentar cualquier riesgo residual si el motor general de
  repaginacion sigue imponiendo limitaciones ajenas a este ticket
- [x] 6.3 Confirmar que el cambio deja desbloqueado un posible refactor mayor
  de paginacion sin contaminar el contrato actual del editor

## Estado de cierre

- Validacion final completada: no se identifican faltantes adicionales dentro
  del alcance de `SCRUMCORE-156`.
- Alcance cumplido: estabilizacion del contador, desacoplamiento del calculo de
  pagina actual, integracion UX discreta, compatibilidad con zoom y ausencia de
  regresion en modo continuo.
- Evidencia focalizada ejecutada: `usePageContext.test.tsx`,
  `AppEditor.test.tsx` y `useAppEditor.test.tsx` con `40/40` pruebas en verde.
- Riesgo residual aceptado: el motor general de repaginacion visual de
  `AppEditor` sigue siendo el actual y aun conserva logica correctiva basada en
  `autoPageBreak`; ese trabajo queda fuera de este ticket.
- Conclusion de cierre: el ticket deja el contador mas estable y menos
  invasivo, y ademas destraba un refactor mayor posterior de scroll/cursor sin
  contaminar el contrato actual del editor.

## Backend

- [x] No aplica: este cambio no requiere contratos HTTP ni integracion backend

## 7. Refactor estructural ejecutado dentro del ticket

- [x] 7.1 Retirar la repaginacion destructiva de `useAppEditor` como mecanismo
  principal del modo visual
- [x] 7.2 Mover el layout visual paginado a una capa derivada de
  `usePaginationMetrics` sin reescribir el documento TipTap en cada ciclo
- [x] 7.3 Confirmar mediante pruebas que el modo visual ya no inserta
  `autoPageBreak` para sostener la experiencia paginada

## Estado de cierre actualizado

- El ticket ya no deja pendiente el ajuste estructural principal que seguia
  abierto durante la validacion anterior.
- `useAppEditor` deja de depender de insercion/limpieza correctiva de
  `autoPageBreak` como comportamiento base del modo visual.
- La paginacion visual queda derivada del layout y del enmascaramiento del gap
  entre hojas, con empuje visual de bloques indivisibles cuando corresponde.
- Evidencia focalizada actualizada:
  `usePaginationMetrics.test.tsx`, `usePageContext.test.tsx`,
  `useAppEditor.test.tsx` y `AppEditor.test.tsx` con `45/45` pruebas en verde.
- Riesgo residual actualizado: el editor todavia no alcanza un comportamiento
  equivalente a Word para todos los bloques complejos, pero el flujo principal
  ya no depende del motor destructivo anterior.

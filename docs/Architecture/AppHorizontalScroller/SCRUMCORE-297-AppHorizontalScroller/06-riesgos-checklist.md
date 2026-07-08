# 06 Riesgos, Restricciones Y Checklist

## Restricciones

- No importar `axios`.
- No llamar `fetch`.
- No importar servicios HTTP.
- No importar hooks o módulos de negocio.
- No depender de `AppTable` ni `AppTreeTable`.
- No agregar dependencias nuevas.
- No usar `dangerouslySetInnerHTML`.

## Decisiones Técnicas

1. Componente puro sin estado ni efectos.
   - Mantiene el primitive enfocado en layout.

2. Custom properties para dimensiones.
   - Evita mutar o clonar `children`.

3. Región accesible nombrada.
   - Un área scrolleable debe tener nombre navegable por tecnologías asistivas.

4. Sin dependencia de Ant Design.
   - El componente es un primitive bajo; los consumidores pueden renderizar componentes Ant Design dentro.

5. Snap con `proximity`.
   - Evita experiencia rígida en listas largas.

## Riesgos Y Mitigaciones

| Riesgo | Mitigación |
|---|---|
| Scrollbars nativos varían por navegador. | Evitar custom scrollbars y apoyarse en comportamiento nativo. |
| Edge fade puede bloquear interacción si se implementa como overlay interactivo. | `pointer-events: none`. |
| Consumidores pueden esperar botones de navegación. | Documentar prev/next como fuera de alcance para esta versión. |
| El ancho aplica a hijos directos. | Documentar el contrato visual y probar custom properties. |
| Un consumidor puede pasar CSS string arbitrario para dimensiones. | Ignorar strings vacíos y strings que empiezan por `-`; permitir valores CSS avanzados no negativos para flexibilidad. |
| No hay screenshot responsive automatizado. | La validación responsive queda documentada por contrato CSS y tests de clases base; la validación visual real corresponde al primer consumidor de pantalla. |

## Checklist De Aceptación

- [x] SCRUM ID documentado.
- [x] Componente UI compartido creado.
- [x] API tipada y defaults definidos.
- [x] Scroll horizontal nativo.
- [x] Región accesible con `aria-label`.
- [x] Densidades y gaps.
- [x] Dimensiones por custom properties.
- [x] Scroll snap opcional con proximity.
- [x] Edge fade no bloqueante.
- [x] Tests unitarios.
- [x] Sin consumo HTTP.
- [x] Sin acoplamiento a dominio.
- [x] Sin cambios en `AppTable` ni `AppTreeTable`.
- [x] Documentación enterprise separada por tema.
- [x] Diagramas de arquitectura, DOM, layout e integración futura.
- [x] Ejemplos de uso.
- [x] Evidencia de validación documentada.

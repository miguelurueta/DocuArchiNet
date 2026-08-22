<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04 -->
# Diseño — DOC-30 Verificación transversal de Enviar a usuario

## Contexto

DOC-30 revisa el snapshot integrado de la capacidad moderna entregada por DOC-28 y DOC-29. La superficie es ASP.NET Web Forms/VB.NET, ASMX y JavaScript legado. El ticket es una compuerta de calidad: sus entradas son el código versionado, las pruebas locales y la evidencia ya saneada; sus salidas son un dictamen técnico y documentación trazable.

## Decisiones

| ID | Decisión | Justificación |
| --- | --- | --- |
| D-01 | La verificación ejecuta únicamente compilación, pruebas locales, inspección estática y QA visual no autenticada; no muta Workflow ni ambientes. | Separa la evidencia de calidad de cualquier autorización E2E, carga o despliegue. |
| D-02 | La revisión del backend se apoya en el contrato directo `usuario–actividad–token`, preview de solo lectura, revalidación bajo lock y auditoría sanitizada. | Evita que una verificación visual sustituya los controles de autorización y concurrencia del servidor. |
| D-03 | La revisión de UI verifica búsqueda paginada, accesibilidad, bloqueo durante envío e independencia de Grupo y Continuar flujo. | La ruta moderna de usuario no puede reintroducir postback Web Forms, `IdConector` ni listeners compartidos. |
| D-04 | La salida de DOC-30 es una recomendación técnica única para la operación posterior, sin desplegar ni editar configuración. | La aprobación de pruebas no equivale a autorización por ambiente y mantiene la reversibilidad operativa. |

## Flujo de verificación

```text
snapshot versionado
  -> inspección estática de contratos y aislamiento
  -> pruebas CJS focales + compilación MSBuild
  -> revisión QA visual no autenticada
  -> matriz de evidencia y riesgos sanitizada
  -> dictamen técnico para la etapa operativa
```

## Riesgos y límites

- Las pruebas locales no sustituyen una autorización operativa por ambiente.
- La concurrencia mutante y la carga no se ejecutan en DOC-30; la evidencia histórica autorizada se cita solo como antecedente, sin repetirla.
- Un fallo de compilación, contrato, aislamiento o accesibilidad produce un ticket de corrección específico y bloquea la recomendación operativa.

## Trazabilidad

Las decisiones D-01 a D-04 se reflejan en los requisitos RQ-01 a RQ-04 y en tareas con su origen explícito.

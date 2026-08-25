<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05 -->
# Diseño técnico — DOC-34

## Contexto

DOC-32 implementó el contrato de devolución de actividad y DOC-33 su interfaz moderna. DOC-34 verifica transversalmente ambos entregables sin alterar el ambiente. La evidencia proviene de compilación local, pruebas CJS/VB, análisis estático, revisión de contratos y QA manual no autenticada.

## Objetivos

- Probar con evidencia reproducible la seguridad del preview y la ejecución de devolución.
- Confirmar el aislamiento de la UI moderna respecto del feature gate y de rutas Web Forms heredadas.
- Proteger las transiciones vecinas mediante una comparación explícita de contratos y pruebas.
- Documentar una recomendación clara para la fase 04 con resultados saneados.

## Fuera de alcance

- Cambiar código de producción, archivos de configuración, esquemas, datos, auditoría o contratos.
- Ejecutar E2E autenticada, carga, despliegue, archivo, publicación o liberación automática.
- Corregir una brecha encontrada dentro de este cambio; esa brecha requiere un ticket de corrección.

## Decisiones

### D-01 — Evidencia local, no mutante y sin secretos

DOC-34 ejecutará únicamente compilación local, suites CJS/VB, análisis estático y QA manual no autenticada. Los comandos se seleccionan para que no llamen endpoints mutantes ni persistan cambios funcionales. Los reportes omiten credenciales, cookies, URL de conexión e identificadores operativos.

**Racional:** la verificación debe ser reproducible sin usar el ambiente como mecanismo de prueba.

### D-02 — Contrato del preview como lectura de conectores entrantes

La revisión cotejará `PreviewDevolverActividad`, `ServicioDevolverActividad` y `MySqlDevolverActividadRepository` con las pruebas focales. Debe comprobar filtro por universo autorizado, fuente Ruta/Flujo entrante, semántica aislada de `IdConector`, orden, cursor, límite y ausencia de escritura. Un conector saliente no es un destino válido de devolución.

**Racional:** distingue la devolución de las transiciones ordinarias y evita revelar o aceptar destinos fuera del historial autorizado.

### D-03 — Ejecución revalidada bajo exclusión por tarea

La revisión cotejará permiso, token vigente, lock exclusivo, consulta del conector entrante, auditoría y política de notificación/eventos. La evidencia de concurrencia de DOC-32 se acepta como antecedente; DOC-34 no vuelve a ejecutar la transición real.

**Racional:** conserva la garantía de una sola transición efectiva sin alterar una tarea de prueba.

### D-04 — UI moderna sin gate ni ruta heredada alcanzable

La revisión estática y CJS verificarán el encadenamiento preview → selección → confirmación, el bloqueo durante una respuesta pendiente, los mensajes de error y atributos de accesibilidad. El marcado Web Forms no debe ofrecer postback, handler o fallback alcanzable para esta operación, ni el código UI debe evaluar `WorkflowCentroTrabajoModernActive`.

**Racional:** elimina bifurcaciones funcionales dependientes del gate y previene doble interacción del usuario.

### D-05 — No regresión, documentación y decisión de salida

La verificación comparará contratos y pruebas de Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior. Los resultados se incorporarán en los índices y la evidencia técnica de `DebolverTarea`, con una recomendación única para fase 04.

**Racional:** la liberación depende tanto de la devolución como de que no cambien los flujos próximos.

## Riesgos y tratamiento

| Riesgo | Tratamiento | Resultado que bloquea |
| --- | --- | --- |
| Una comprobación local no cubre el ambiente real. | Declarar la exclusión de E2E/carga y enlazar la evidencia aprobada de DOC-32/DOC-33. | Falta de evidencia contractual o de concurrencia previa. |
| Un análisis estático no detecta una ruta heredada dinámica. | Cruzar scripts, marcado, código detrás y CJS de política. | Handler, postback o gate alcanzable para devolver. |
| Una diferencia con transiciones vecinas puede romper compatibilidad. | Ejecutar y revisar pruebas de regresión focales antes de recomendar fase 04. | Contrato o prueba vecina alterada. |
| Un hallazgo conduce a cambiar producción para hacer verde la suite. | Registrar una corrección separada y conservar DOC-34 como evidencia. | Cualquier cambio funcional requerido. |

## Plan de verificación

1. Confirmar que el worktree funcional no recibe cambios y que no se usan credenciales.
2. Ejecutar compilación disponible, suites CJS/VB focales y análisis estático de los símbolos indicados.
3. Realizar QA manual no autenticada de interfaz y accesibilidad, documentando cobertura y límites.
4. Comparar contratos de transición vecinos, actualizar documentación y formular la recomendación de fase 04.

## Migración y reversión

No existe migración ni reversión funcional: DOC-34 solo agrega evidencia y documentación. Si una comprobación falla, se conserva el resultado saneado y se abre una corrección; no se modifica la implementación para alterar el diagnóstico.

## Cuestiones abiertas

No existen cuestiones abiertas que impidan iniciar los controles locales definidos.

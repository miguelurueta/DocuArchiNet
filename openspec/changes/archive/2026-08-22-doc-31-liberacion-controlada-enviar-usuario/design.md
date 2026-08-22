<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# Diseño — DOC-31 Liberación controlada de Enviar a usuario

## Contexto

La versión integrada está en `main` mediante el merge del PR #23 y DOC-30 certificó compilación, pruebas, inspección y QA visual. Jira no identifica ambiente objetivo, ventana aprobada ni responsables nominales de operación. Por ello la única decisión compatible es solicitar aprobación operativa, no desplegar.

## Decisiones

| ID | Decisión | Justificación |
| --- | --- | --- |
| D-01 | La decisión de DOC-31 es `solicitar aprobación operativa`. | La evidencia técnica está aprobada, pero falta autorización explícita por ambiente. |
| D-02 | La versión de referencia es `main` en el merge `43d42045beea0984c1b63193e66d12f6a49e5e1c` del PR #23. | Identifica un artefacto versionado sin asociarlo a un despliegue. |
| D-03 | La matriz declara cero ambientes elegibles hasta que una solicitud de operación nombre ambiente, ventana, responsables por rol y aprobador. | Evita inferir autorización entre ambientes o desde pruebas anteriores. |
| D-04 | El runbook solo permite verificaciones documentales o consultas `SELECT` autorizadas y sanitizadas. | Mantiene el principio de mínimo privilegio y evita mutaciones durante preparación. |
| D-05 | La reversión se realiza mediante el mecanismo de despliegue aprobado hacia el paquete previo; no revierte tareas ni respuestas confirmadas. | La reversibilidad aplica a nuevos intentos, no al historial de Workflow. |
| D-06 | La liberación preserva la ruta moderna oficial de usuario y el contrato `IdConector` de Continuar flujo. | No se permite reactivar postback legacy ni alterar flujos ajenos. |

## Flujo operativo futuro

```text
solicitud por ambiente y roles
  -> validar versión y evidencia DOC-30
  -> aprobar ventana y plan de reversión
  -> desplegar mediante gestión aprobada
  -> verificar con SELECT sanitizado
  -> continuar o abortar/revertir el paquete
```

## Riesgos y límites

- Ningún ambiente está autorizado por este ticket; la matriz no habilita una operación.
- Las consultas y evidencias solo pueden ejecutarse con aprobación del ambiente correspondiente.
- Un incidente posterior se trata con el proceso de despliegue y soporte vigente, sin alterar transiciones de Workflow ya confirmadas.

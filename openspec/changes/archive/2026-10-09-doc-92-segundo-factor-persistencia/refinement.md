<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-92 Persistencia compatible del segundo factor

## Fuente y alcance

- Ticket: `DOC-92`.
- Cambio: `doc-92-segundo-factor-persistencia`.
- Perfil: VB.NET WebForms, MySQL y ADO.NET compartido.
- Contrato definitivo: reutilizar la tabla que ya utiliza DocuArchiCore sin crear tablas, columnas o índices.
- Exclusiones: ASMX, UI, SMTP, activación funcional, DDL y contexto HTTP.

## Contexto inspeccionado

- `MiApp.Repository/Repositorio/Autenticacion/SecondFactorChallengeRepository.cs` del repo nuevo.
- `MiApp.Models/Models/Auntetnicacion/AuthSecondFactorChallengeDA.cs` del repo nuevo.
- `EmailSecondFactorProvider.cs` y `SecondFactorService.cs` del repo nuevo.
- Exploración versionada y esquema físico previamente inspeccionado.
- Fundación DOC-91 y abstracciones ADO.NET locales.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Persistencia interna e inactiva. | Estructura local | D-01 | RQ-01 | 2.4 |
| D-02 | Usar exactamente las diez columnas existentes; cero DDL. | Modelo `AuthSecondFactorChallengeDA` | D-02 | RQ-02 | 2.1 |
| D-03 | Preservar los dos índices actuales sin agregar otros. | Inspección física | D-03 | RQ-03 | 2.1a |
| D-04 | Inyectar conexión, executor, transacción, contexto y reloj. | `ModuleDataContracts.vb` | D-04 | RQ-04 | 3.2 |
| D-05 | Guardar identidad canónica y HMAC en campos existentes; payload nulo. | Fundación DOC-91 | D-05 | RQ-05 | 1.1, 1.2 |
| D-06 | Incrementar y consumir con bloqueo y actualización condicionada. | Riesgo observado en repo nuevo | D-06 | RQ-06 | 2.2, 3.3 |
| D-07 | Derivar condición desde consumo/intentos/expiración y aceptar límites del esquema. | Contrato físico | D-07 | RQ-07 | 2.3, 4.1 |
| D-08 | No entregar apply, rollback ni cleanup porque no hay cambio físico. | Decisión explícita del usuario | D-08 | RQ-08 | 3.1 |
| D-09 | Pruebas locales siempre; MySQL real solo autorizado. | Reglas del repo | D-09 | RQ-09 | 3.4, 4.2, 4.3 |
| D-10 | Documentación y diagramas verificables estructuralmente contra VB.NET. | Solicitud arquitectónica | D-10 | RQ-10 | 5.1, 5.2, 5.3, 5.4 |

## Requisitos verificables

| ID | Resultado observable | Criterio | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Login actual intacto. | Sin consumidores productivos ni endpoints. | Evita activación accidental. |
| RQ-02 | Contrato físico idéntico. | INSERT/SELECT solo usan diez columnas; no hay DDL. | Compatible con DocuArchiCore. |
| RQ-03 | Índices intactos. | No aparece CREATE/DROP INDEX. | Rendimiento limitado al esquema actual. |
| RQ-04 | Infraestructura desacoplada. | Sin contexto HTTP, helper legacy o SQL concatenado. | Conexión central explícita. |
| RQ-05 | Sin login o contexto sensible persistido. | AuthUserId canónico, HMAC en CodeHash, payload nulo. | DocuArchiCore no finaliza filas creadas por DocuArchiNet. |
| RQ-06 | Un solo consumo y quinto intento estable. | FOR UPDATE más UPDATE condicionado. | MySQL real requiere prueba autorizada. |
| RQ-07 | Solo se representan señales soportadas. | Activo/bloqueado/expirado/consumido derivados. | No existe estado finalization-failed persistido. |
| RQ-08 | Despliegue sin migración. | Carpeta SQL contiene únicamente explicación. | Sin rollback de esquema necesario. |
| RQ-09 | Evidencia honesta. | Local verde; integración omitida sin autorización. | No atribuir aislamiento real a dobles. |
| RQ-10 | Documentación trazable al código. | Inventario explícito, Mermaid y Roslyn en CI. | La estructura no prueba fidelidad conductual completa. |

## Reglas de trazabilidad

1. Cada D-XX aparece en diseño, especificación y tareas.
2. No se versionan secretos ni se accede a una base real sin autorización vigente.
3. Una futura necesidad de más estado requiere una decisión coordinada entre ambos repositorios, no una migración unilateral.

## Resultado

- Estado: aprobado e implementado bajo contrato sin DDL.
- Validación: OPSXJ refinement y OpenSpec strict.

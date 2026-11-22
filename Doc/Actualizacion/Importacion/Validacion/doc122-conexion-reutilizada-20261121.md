# DOC-122 — Conexión DA reutilizada

Corte local anterior a la ejecución real. La aceptación real posterior está en [doc122-e2e-real-20261122.md](doc122-e2e-real-20261122.md):2/2 recorridos aprobados y14/14 tareas. Las menciones de integración abierta que siguen describen este corte histórico.

Estado **PASS_LOCAL_REAL_PENDING**. Revisión solicitada por el usuario tras preguntar por qué el importador no utilizaba los recursos de conexión existentes y ordenar continuar. Sustituye el requisito inicial de una cuenta adicional y una conexión Excel en Web.config. La evidencia anterior de configuración protegida se conserva como histórica.

## Resultado vigente

| Comprobación | Resultado |
|---|---|
| Build del proyecto .NET Framework4.6.1 en salida aislada | PASS, exit0. |
| Suite nativa DOC122 | PASS,246 controles sintéticos. |
| Regresión DOC110/DOC120 del harness existente | PASS. |
| Adaptación del runner E2E existente |6/6 PASS locales. |
| Playwright, solo descubrimiento |2 recorridos;0 ejecutados. |
| OpenSpec | PASS estricto, ambos deltas. |
| Refinement |15/15 PASS. |
| DLL activa y Web.config antes/después del build | Sin cambios. |

Suite completa: node --test tests/doc122-importacion-excel-behavior.test.cjs, aproximadamente40,8s. Incluye compilación de fuentes nuevas, ejecución nativa y runner de regresión existente. Verificación adicional del ejecutable nativo confirma246 controles. Build: MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build, Configuration=Debug, OutDir/BaseIntermediateOutputPath/IntermediateOutputPath dentro de .tmp/doc122-conexion-build, DeployOnBuild=false, Disable_CopyWebApplication=true y UseSharedCompilation=false. No se acredita empaquetado completo ni despliegue.

## Cambio de conexión
El adaptador llama ModuleSessionConnectionStringResolver.Resolve(http,"DA_"), igual que la importación de carpetas. La cuenta procede de gestor_modulos a través del cargador de sesión existente; no se modifican esos recursos compartidos. La apertura conserva usuario, contraseña efímera del fixture y opciones de pool; deshabilita persistencia de seguridad y variables de usuario del conector.

Se acredita con dobles ADO.NET: empresa ajena, módulo ausente/0, proveedor incompatible, contraseña de sesión ausente, ID histórico ajeno, host/base/proveedor histórico distintos, diferencias ordinales de base y duplicidad del catálogo rechazados. Catálogo consultado por parámetros, sin columnas de credenciales. Si cambia módulo o configuración de sesión después de componer el repositorio, la fábrica rechaza antes de abrir otra conexión. La respuesta solo contiene ID/nombre. Límites positivos/defaults se mantienen sin configuración de conexión adicional.

La opción comparte la cuenta del módulo DA; no certifica separación física de privilegios para Excel. No hay concesiones, revocaciones, altas de cuenta, DDL o DML de instalación. Los scripts SQL históricos se conservan; SQL05 deja de ser requisito para una cuenta separada. La decisión vigente y su delta DOC121 documentan esta revisión del contrato.

## Límites de evidencia
246 controles ejecutan las fuentes del adaptador y repositorio reales sobre fixtures sintéticos; no certifican permisos efectivos, bloqueo MySQL ni enrutamiento real de la sesión del sitio. Cero HTTP real, sesiones autenticadas, SELECT real, DDL/DML real, cambios a EGRESOS o a su archivo Excel y despliegues. El Excel aportado permanece local y no se incluye en Git.

El recorrido real sigue abierto y requiere despliegue del binario verificado y autorización específica para ambiente, cuentas y plantilla descartable. El usuario confirmó disponer del permiso funcional; esa confirmación no acredita que se haya ejecutado la prueba. No se necesita configurar una conexión dedicada adicional para este ajuste.

## Trazabilidad
Base de esta revisión: f4238873, rama feature/DOC-122. El informe JSON adjunto identifica SHA256 UTF8/LF de las fuentes efectivamente verificadas y el binario aislado. No se utiliza el resultado histórico de191 controles como prueba de la implementación nueva. DOC122 permanece13/14 tareas; integración real sin marcar.

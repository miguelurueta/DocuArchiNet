# DOC-122 — Validación local, 2026-11-21

Evidencia histórica del mecanismo inicial de configuración protegida. La revisión vigente de conexión usa los recursos del módulo y está en [doc122-conexion-reutilizada-20261121.md](doc122-conexion-reutilizada-20261121.md), con246 controles y nuevo build. Las afirmaciones de cuenta adicional que siguen describen el corte anterior.

Estado **PASS_LOCAL_REAL_PENDING**. Avance13/14 tareas; integración real abierta. Fuentes y huellas normalizadas UTF8/LF en [el informe JSON](doc122-validacion-local-20261121.json). Base Git de la prueba04e47dada8d85ef216cf765771b452de98aa56a9; la identidad del código probado se conserva por archivo y digest del conjunto, sin fingir un SHA final aún no creado.

## Resultado
| Control | Resultado |
|---|---|
| MSBuild proyecto .NET Framework4.6.1, Debug, salida aislada | PASS, exit0. |
| Suite nativa DOC122 | PASS,191 controles sobre fixtures sintéticos. |
| Regresión DOC110/DOC120 existente | PASS, ejecutada por el mismo harness. |
| Snapshot v1, vector ASCII/NULL/ordinal | PASS. |
| Reflexión SOAP ASMX y respuestas tipadas | PASS. |
| Refinement |15/15 PASS. |
| OpenSpec del cambio | PASS estricto. |
| Playwright, solo descubrimiento |2 recorridos encontrados;0 ejecutados. |

Build usa OutDir/BaseIntermediateOutputPath/IntermediateOutputPath dentro de .tmp/doc122-project-build, DeployOnBuild=false, Disable_CopyWebApplication=true y UseSharedCompilation=false. Comprobación antes/después: DLL activa y Web.config idénticos. El primer Build sin desactivar copia compiló y falló al empaquetar18 contenidos antiguos ausentes; el build de código aislado pasó. No se acredita publicación ni integridad del paquete web completo.

Comando principal: node --test tests/doc122-importacion-excel-behavior.test.cjs. La última suite completa terminó en47,9s aproximadamente; una ejecución nativa focal posterior confirmó191 controles. Reutiliza compileBehaviorHarness DOC110, compila las fuentes nuevas, prueba DOC122 y ejecuta el runner de regresión existente. Archivos de salida únicamente en .tmp; sin sustituir DLL del sitio.

## Conductas acreditadas localmente
Permiso propio sin exigir administración de perfiles; usuario inactivo, empresa ajena, duplicidad de autoridad y permiso revocado rechazados. Configuración real de .NET protegida con valores ficticios: texto plano rechazado, descifrado local, vínculos válidos, IDs ajenos y DA diferente rechazados, límites inválidos cerrados. Ninguna conexión MySQL se abrió: las fábricas ADO.NET son sintéticas.

Repositorio real ejecutado contra dobles ADO.NET: SQL parametrizado, lectura por conexión, revisión incremental, un ganador entre solicitudes concurrentes, conflicto por editor antiguo sin subir revisión, guardado negado para catálogos MyISAM, y rollback de cabecera más el primer mapeo tras fallo en el segundo. También conservación de rutas, IDs, NULL y longitudes físicas ampliadas; alta con ruta relativa generada; flags y tipos seguros. No simula todas las restricciones del motor MySQL.

CSRF de propósito Excel vinculado a sesión/empresa, transporte/origen forjados rechazados y token estable para varias pestañas. Reflexión ASMX nativa acredita que los contratos tipados son exportables; no certifica que el endpoint esté desplegado en IIS. Vector de huella estable aun con JsonConvert.DefaultSettings global diferente.

## Límites y pendientes
Cero HTTP real, sesiones autenticadas, SELECT real, DDL/DML real o despliegues en esta fase. EGRESOS y sus datos permanecen intactos. SQL10 entregado solo para verificación SELECT; no fue aplicado ni ejecutado por esta validación.

Queda pendiente configurar/comprobar la cuenta dedicada protegida y autorizar ambiente, cuentas y plantilla existente descartable para E2E. Los recorridos de tools/e2e reutilizan autenticación y safePost. La prueba de escritura prevé tres solicitudes funcionales: dos éxitos y un conflicto, revisión+2, sin cambiar valores/rutas/mapeos ni ejecutar SQL desde el test. Una misma sesión ASP.NET serializa peticiones; ese recorrido no acredita paralelismo físico de dos escritores MySQL.

Los controles externos del ambiente deben ser exclusivamente SELECT y acreditar catálogos/destino/auditoría preservados. No se inventan QA, concesiones, instalación ni enrutamiento RA/DA a partir de resultados sintéticos. No se registra manual_qa como aprobada. El gate de cierre permanece abierto por esa razón y por la revisión final correspondiente al futuro SHA de entrega.

## Operación
Actualización de reutilización E2E: el runner existente run-doc110-profile-session.cjs incorpora --excel-read y --excel-write. Comparte perfil local DOC88, autenticación, consola efímera, safePost, Playwright y selección de cuentas DOC110; conserva alcance y autorización específicos DOC122. Node aprobó6 verificaciones locales (aliases, prioridad explícita, modos incompatibles, ambiente ajeno, lectura exclusiva e identidad/revisión descartable). Descubrimiento Playwright conserva2 recorridos. Cero sesiones, HTTP o SQL reales. Los16 inputs originales mantienen sus huellas; no se repitió el build ni la suite nativa porque no cambió código VB/DTO/repositorio ni sus pruebas.

Guía y contrato en [02-autorizacion-plantillas-doc122.md](../Diseno/02-autorizacion-plantillas-doc122.md). Una falla GUARDADO_NO_COMPLETADO exige consultar antes de repetir; nunca relanzar automáticamente una ventana con resultado incierto. Los editores antiguos no obedecen el bloqueo de aplicación después del commit: las fases posteriores deben revalidar huellas.

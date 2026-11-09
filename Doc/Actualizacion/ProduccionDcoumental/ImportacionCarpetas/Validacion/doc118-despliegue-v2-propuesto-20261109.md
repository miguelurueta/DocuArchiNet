# DOC-118 — propuesta concreta de despliegue del candidato V2

Estado: **PROPUESTO, SIN AUTORIZAR NI EJECUTAR**. Paso P4-A de la tarea 4.1; no agrega tareas ni completa integración. Fuente de código `9d67c81f27cb714412d17772ad492b56968407a3`; compilación completa de 920 archivos con MSBuild Debug, salida 0. El candidato ordinario no incluye el decorador de pérdida de acuses. El [manifiesto de alcance](../../../../../openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/v2-candidate-deployment-proposal.json) fija los hashes completos y los límites; los artefactos locales ignorados se conservan para ejecución/revisión.

Destino IIS comprobado por lectura de configuración: `Default Web Site/GestionDocumental-Docuarchi.net`, URL `https://localhost/GestionDocumental-Docuarchi.net/`, raíz física igual al repositorio principal. Pool `DefaultAppPool` compartido por 6 aplicaciones: **no se recicla ni detiene ese pool**. La inspección de configuración fue elevada tras denegación de permisos; no inició sesión, envió HTTP o verificó un módulo cargado. Las consultas iniciales sin privilegios no se toman como prueba de ausencia del sitio.

| Archivo | Candidato local | Destino único |
| --- | --- | --- |
| GestionDocumental-Docuarchi.net.dll | `.tmp/doc118-v2-candidate-9d67c81f/bin/GestionDocumental-Docuarchi.net.dll` | `bin/GestionDocumental-Docuarchi.net.dll` |
| GestionDocumental-Docuarchi.net.pdb | `.tmp/doc118-v2-candidate-9d67c81f/bin/GestionDocumental-Docuarchi.net.pdb` | `bin/GestionDocumental-Docuarchi.net.pdb` |

Se propone una ventana sin importaciones activas y mantenimiento solo de esta aplicación durante la copia (objetivo 60 s). Primero se revalidan ruta IIS, proyecto/fuentes/artefactos y hashes instalados. Se crea respaldo nuevo verificado de ambos archivos bajo `.tmp/doc118-v2-deploy-9d67c81f-20261109-01/backup`; no se sobrescribe un respaldo anterior. Se usa `app_offline.htm` temporal solo si no existe, se copian exactamente los dos archivos y se comprueban hashes/símbolos. Se retira únicamente el archivo de mantenimiento propio al confirmar ambos. El reemplazo causa recarga del AppDomain de esta aplicación; puede perder sesiones y época del proveedor, por eso la ventana de mantenimiento requiere aprobación. No se mata un proceso ni se reinicia IIS.

Si una copia o hash falla, se restauran solo esos dos archivos desde el respaldo verificado. Si no se confirma restauración, se conserva el mantenimiento y se informa; no se publica una instalación parcial como correcta. No se copian dependencias/Web.config ni se toca almacenamiento/SII. No se instalan hooks de fallo, endpoints o controles de prueba.

Presupuesto de este paso: **0 autenticaciones, 0 HTTP, 0 SELECT, 0 creaciones/almacenamientos/recuperaciones**. No se solicita todavía ejecutar Q1..Q4/COORD02/03 ni conectar observador. Los topes históricos están agotados para aperturas y no se reutilizan. Tras el despliegue deberán calificarse módulo cargado/proveedor/MySQL/IIS/observador y enlace/lector del host bajo un alcance separado revisable. P3 no está instalado; P4/P5 siguen abiertos. El reemplazo por sí solo no valida la aplicación en ejecución.

El alcance autorizado previo fue solo código/pruebas locales. El [plan real V2](doc118-plan-real-v2-20261109.md), dependencias4/5, exige autorización específica de despliegue/calificación; `v2-ack-loss-conductor-authorization.json` mantiene `deploymentAuthorized=false`. Se solicita aprobación únicamente después de compilar, identificar y fijar estos artefactos. Progreso 28/38; mismas 10 tareas pendientes.

# DOC-118 — despliegue autorizado del candidato V2

**DLL/PDB instalados y verificados en disco; runtime todavía sin calificar.** Propuesta revisada en `7a1444ff7d76a9fd501e9ec2b70c327c947495ac`, código compilado de `9d67c81f27cb714412d17772ad492b56968407a3`. El usuario respondió «autoriozado» a la solicitud concreta de desplegar esos dos archivos con respaldo y mantenimiento breve de esta aplicación. Esta aprobación no incluye ejecución real de Q1..Q4/COORD02/03.

Destino: `Default Web Site/GestionDocumental-Docuarchi.net`, URL `https://localhost/GestionDocumental-Docuarchi.net/`, raíz física revalidada igual al repositorio principal. Pool `DefaultAppPool` compartido por 6 aplicaciones; no se reinició/recicló ni se detuvo un worker. Se revalidaron las 920 fuentes del build y el proyecto, los artefactos y los archivos instalados antes de copiar.

| Archivo | SHA256 instalado |
| --- | --- |
| bin/GestionDocumental-Docuarchi.net.dll | `9225b627c50b96c29ee9031173f0c578df4a16e970cb45404ee6be4d13eff291` |
| bin/GestionDocumental-Docuarchi.net.pdb | `37aab89e1a02ed2704644ec9d58e6f44f19125e36f1b8bdc60d3f7b8f4f97b72` |

Ambos archivos originales se copiaron a `.tmp/doc118-v2-deploy-9d67c81f-20261109-01/backup/` y sus hashes coinciden con los esperados en la propuesta. El respaldo permanece local y excluido de Git. Se creó exclusivamente el `app_offline.htm` propio, se copiaron solo DLL/PDB y se verificaron hashes y correspondencia CodeView/PDB. El mantenimiento se retiró tras `627 ms`; no queda archivo offline. No fue necesaria restauración. La sustitución permite la recarga automática del AppDomain, pero no se inspeccionó su carga efectiva ni se enviaron solicitudes para calentar la aplicación.

La lectura administrativa de peticiones activas devuelve `0x80004001` (no implementada) en esta configuración IIS. Se usó la ventana sin importaciones activas aprobada por el operador, prevista en la propuesta; **no se afirma que IIS haya demostrado cero solicitudes ni drenado la ejecución**. No se activó/configuró un módulo de monitorización para resolverlo. Los controles extra fallidos ocurrieron antes de crear mantenimiento o copiar archivos.

Web.config, ClassAlmacenamiento.vb y el binding legacy/InvocarAlmacenamiento mantienen sus hashes. No se cambió fuente de producto/SII, configuración, dependencias, permisos o tablas ni se instaló el decorador/host de prueba. No se ejecutó almacenamiento ni recuperación. Incremento de este paso: 0 login, 0 HTTP, 0 SELECT, 0 efectos; consumo histórico 1276/113/9 intacto. El resultado de archivos no acredita salud HTTP, módulo/símbolos cargados, proveedor, MySQL o llamada observada.

El runner histórico `doc118-live-plan.cjs` compara el binario con el corte antiguo `doc118-preflight-20261107.json`; esa referencia se preserva. Su guard ORDINARY_BINARY_CHANGED bloqueará una ejecución con el candidato nuevo hasta revisar un nuevo binding y su calificación. No se altera un informe histórico para eludirlo. No se autoriza ni se intenta otra sesión con presupuestos agotados.

Progreso 28/38; no agrega tareas ni completa 4.1. P3 sigue solo preparado localmente, P4 conserva la calificación pendiente y P5 no autoriza casos reales. [Estado posterior del plan](doc118-plan-real-v2-despliegue-20261109.md). La regresión 247/247 de 6.11 y el build completo previo se conservan con sus hashes y no se presentan como nuevas corridas de esta instalación.

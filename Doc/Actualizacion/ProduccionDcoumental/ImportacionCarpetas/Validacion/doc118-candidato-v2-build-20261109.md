# DOC-118 — compilación completa y preparación de despliegue V2

Base `9d67c81f27cb714412d17772ad492b56968407a3`, fuente principal limpia al compilar. **PASS de compilación offline; runtime sin calificar y sin despliegue.**

MSBuild del proyecto real .NET Framework 4.6.1 terminó con salida 0; 920 fuentes declaradas en el proyecto permanecieron idénticas antes/después, al igual que el proyecto. Se usó Debug/AnyCPU, DeployOnBuild=false y PublishDatabases=false, con OutputPath/OutDir/objetos aislados en `.tmp/doc118-v2-candidate-9d67c81f/`. El manifiesto JSON conserva comando, agregado de fuentes y hashes. El cotejo CodeView/PDB es concordante para candidato e instalación ordinaria; no acredita símbolos cargados o pausa interna.

DLL candidata: `9225b627c50b96c29ee9031173f0c578df4a16e970cb45404ee6be4d13eff291`. DLL instalada conservada: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. Son distintas: la compilación no constituye publicación en IIS. Los binarios instalados, Web.config, función compartida y binding legacy permanecieron idénticos por SHA256; 0 llamadas a almacenamiento y 0 HTTP/SQL/login. No se cambió producto ni tests; se reutiliza la regresión 247/247 Node de la entrega 6.11, sin atribuir una corrida nueva.

La inspección elevada y de solo lectura confirmó la aplicación IIS y su raíz física igual al repositorio principal. El pool aloja6 aplicaciones: no reciclarlo por inferencia. El candidato se limita a DLL/PDB ordinarios, sin decorador o host de prueba. Propuesta concreta de respaldo, mantenimiento por aplicación, copia/hash y restauración en [despliegue propuesto](doc118-despliegue-v2-propuesto-20261109.md).

Este corte apoya 4.1/D-10/RQ-10 y D-01/RQ-01; no agrega otra tarea, no marca 4.1 completa ni afirma calificación del proveedor, MySQL o IIS.28/38, mismas 10 pendientes. Quedan P3 instalado/calificado, P4/P5 y evidencia COORD02/03. Consumo real histórico 1276 HTTP/113 SELECT/9 sesiones sin cambios. La propuesta requiere aprobación específica antes de cualquier reemplazo o mantenimiento del sitio.

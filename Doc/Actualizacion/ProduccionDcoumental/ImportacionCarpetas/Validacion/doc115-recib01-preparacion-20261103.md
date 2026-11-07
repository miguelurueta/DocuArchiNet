# DOC-115 — RECIB-01 listo para autorización específica

Se corrigió el nombre de consulta a ConsultarConfirmacionLote según la fachada ASMX. Una comprobación local contrasta todas las operaciones del cliente con los métodos públicos existentes. También se exige ultimaModificacionLocal decimal y se consulta el inventario de planes anteriores antes de preparar: si hay nombres coincidentes se registra el plan y se detiene.

Regresión posterior: 78/78 Node PASS. El harness Framework anterior conserva 2/2 PASS y no se repite por este cambio JavaScript. Ninguna prueba autenticada ni SQL ejecutados. El PDF reutilizado es doc111-carga-individual.pdf, 677 bytes, con hash coincidente entre repo raíz y worktree; no se sobrescribe la fixture DOC-110 cuyo contenido difiere entre copias.

Recursos propuestos: ambiente local de pruebas https://localhost/GestionDocumental-Docuarchi.net/, cuenta operativa 141/empresa 2, nivel 7, perfil 24/version 38. Dos preparaciones con nombres DOC115-20261103-RECIB01-01/02, hasta un lote y cinco solicitudes de confirmación incluyendo concurrencia/replay/rechazos. Cero recepciones de PDFs, creación de expedientes o almacenamiento definitivo. Controles RA/DA exclusivamente SELECT por los DSN registrados MembershipUsers; no URL MySQL. Perfiles, permisos y configuración no cambian. Los planes/lote nuevos se conservan, al igual que recursos previos.

Archivo de recursos local: .tmp/doc115-recib01-resources.proposed.json. Su huella está en el JSON adjunto. Comando desde el worktree: node tools/e2e/scripts/run-doc115-recepcion-importacion.cjs --session ../../.tmp/doc115-recib01-resources.proposed.json --check. Sin --check requiere autorización específica y credenciales efímeras en la consola existente. Los resultados de pruebas reales se conservan como NO_EJECUTADO hasta esa autorización; el runbook y AGENTS.md impiden inferirla de la aprobación editorial.

El comando RECIB-01 verifica concurrencia HTTP que podría serializar la sesión; no cierra las carreras de conexiones MySQL independientes de 07-A. El arnés antiguo activa perfil y exige tablas vacías: no ejecutarlo de nuevo en este ambiente.

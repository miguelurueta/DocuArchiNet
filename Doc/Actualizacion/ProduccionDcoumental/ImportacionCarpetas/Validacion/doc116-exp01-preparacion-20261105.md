# DOC-116 EXP-01: preparación autorizada

Autorización explícita del usuario: `autorizado`, sobre la propuesta de recursos del 2026-11-05. La copia local es la fuente de revisión.

Un plan, un lote, dos recepciones de PDF de 677 bytes, dos expedientes bajo nivel 7 y cuatro solicitudes de creación/replay. Cambio visual a nivel 8. Cero documentos definitivos y cero cambios de permisos. Se conservan los recursos y planes/lotes anteriores.

La composición utiliza el login y cliente DOC-111 existentes, y el ejecutor de creación DOC-116. No invoca materialización ni almacenamiento. Busca planes previos por los nombres autorizados, comprueba la cuenta mediante el lote 11 antes de preparar y registra cada solicitud antes de enviarla. Una respuesta incierta detiene la sesión, sin reintentos.

Validación local: 79/79 pruebas, incluyendo ocho del setup; Playwright descubre la nueva prueba. No representa aceptación real EXP-01. EXP-02 interno sigue sin composición autenticada y EXP-03 real pendiente; esta autorización no permite revocar permisos.

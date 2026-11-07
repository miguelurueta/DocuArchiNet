# DOC-110 — Alta de fuente sin mensaje visible

Fecha: 2026-10-23. Entrada: grabación local proporcionada por el usuario, de 31 segundos. Se inspeccionaron fotogramas sin incorporar el video al repositorio.

## Diagnóstico

La grabación inicia con un aviso de consultar estado antes de repetir la escritura, situado en la parte superior. Al desplazarse hasta el motivo y Crear fuente inactiva, el aviso queda fuera del área visible. Los clics posteriores pueden detenerse en la protección local por resultado incierto, dando la impresión de que el botón no actúa. El video no contiene evidencia de red que permita atribuir el primer fallo HTTP a un código concreto.

SELECT independiente en RA a 2026-10-23T15:42:16.2463383Z confirma fuente/columna y permiso existentes, pero ra_integracion_fuente_historial sigue ausente. El repositorio necesita esa tabla para confirmar el alta junto con su auditoría. No se ejecutó ninguna creación ni se alteraron datos durante este diagnóstico.

## Corrección

- Aviso de fuentes sticky dentro del diálogo, con región accesible aria-live; permanece visible junto al desplazamiento del formulario.
- Texto más explícito para resultado anterior incierto, sin reintentar automáticamente.
- Preflight SELECT del esquema de historial antes de la primera escritura. Tabla/columnas ausentes generan ESQUEMA_FUENTES_INCOMPLETO y el mensaje de completar SQL 07 en RA.
- La API distingue ese rechazo conocido previo a escritura del resultado incierto de otros errores 503. La excepción está limitada a las cuatro mutaciones de fuentes y a ese código exacto. Timeouts y errores genéricos mantienen la protección.

## Verificación y límites

Build aislado PASS. Suite Node (cliente, editor de fuente, harness .NET) 9 PASS. Playwright doc110-fuentes-guided.spec.cjs 2 PASS: recorrido anterior y nuevo caso a 1000×600, con mensaje de error dentro del viewport al desplazarse al botón, motivo conservado, controles habilitados y una sola solicitud enviada.

El rechazo HTTP de la E2E se simula mediante el cliente real; no se presenta como ejecución de alta autenticada en RA. No se desplegó DLL. La tabla faltante requiere ejecución manual del bloque CREATE TABLE ra_integracion_fuente_historial del SQL 07, sin repetir las otras tablas ni el ALTER de permiso. La corrección de mensajes no sustituye completar esa instalación.

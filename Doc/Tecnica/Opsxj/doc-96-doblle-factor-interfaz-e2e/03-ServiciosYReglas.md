# DOBLLE-FACTOR-INTERFAZ-E2E

- Ticket: DOC-96
- Cambio OpenSpec: doc-96-doblle-factor-interfaz-e2e
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

DOC-96 no crea servicios de negocio ni modifica DOC-95. `gestor.aspx.vb:Button1_Click` continúa invocando `ClassGestorSesion.InicioAplicacionWebGestorDocumental`; `YES` conserva la redirección legacy, los errores mantienen su tratamiento y solo `SECOND_FACTOR_REQUIRED` inicia `DocuArchiSegundoFactor.iniciar()`.

El cliente mantiene los estados `LOGIN`, `CONSULTANDO`, `PENDIENTE`, `SOLICITANDO` y `COMPLETADO`. Solo permite una mutación en vuelo, toma los segundos del servidor como fuente temporal y al llegar a cero consulta nuevamente el estado: nunca autentica ni reenvía por reloj local.

Solo `SECOND_FACTOR_COMPLETED`, estado `COMPLETADO` y una ruta `~/` validada permiten navegar. Esquema, host, traversal, barra inversa, query o fragmento se rechazan. Cancelación, expiración, bloqueo, límite y fallos de transporte, entrega o finalización restauran el login sin autenticar. No se usa storage, consola, query string ni cookies propias.

# VERIFICACION-TRANSVERSAL-DEVOLVER-TAREA

- Ticket: DOC-34
- Cambio OpenSpec: doc-34-verificacion-transversal-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

`PreviewDevolverActividad` recibe tarea, término, cursor y tamaño de página. Publica destinos autorizados, token y cursor sin exponer SQL, sesión ni destino aportado por el navegador.

`EjecutarDevolverActividad` recibe solo tarea, conector y token. El servidor reconstruye Ruta o Flujo y revalida el conector entrante bajo lock. No se modifican DTOs de usuario, grupo o Continuar flujo, ni esquema o configuración del ambiente.

Las respuestas son funcionales y saneadas. La evidencia no contiene credenciales, cookies, cadenas de conexión ni datos de tarea.

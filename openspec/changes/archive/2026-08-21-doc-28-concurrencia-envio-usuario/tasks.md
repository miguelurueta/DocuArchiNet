## 1. Protección de configuración

- [x] 1.1 Extender el validador DOC-28 con el modo `concurrency`, prerrequisitos de MySQL y autorización doble exacta antes de navegador o HTTP.
- [x] 1.2 Registrar el comando npm exclusivo de concurrencia DOC-28, separado de preview, ejecución individual y cualquier carga masiva.
- [x] 1.3 Cubrir estáticamente que el comando falla cerrado sin autorización de concurrencia y que no admite destino ni token configurables.

## 2. Runner de carrera controlada

- [x] 2.1 Implementar el runner DOC-28 de exactamente dos sesiones y solicitudes simultáneas, con destino y token derivados del preview vigente.
- [x] 2.2 Verificar una ganadora `completada`, una perdedora con código permitido, huellas de estado/auditoría y cierre de contextos, pool y navegador.
- [x] 2.3 Generar evidencia agregada sin destinos, tokens, cookies, secretos, cadenas de conexión ni cuerpos de respuesta, y comprobar gate apagado antes/después.

## 3. Documentación y regresión local

- [x] 3.1 Actualizar README y runbook DOC-28 con alcance fijo de dos solicitudes, autorización doble, tarea descartable y prohibición de carga masiva.
- [x] 3.2 Ejecutar pruebas estáticas, validadores incompletos y validación estricta de OpenSpec sin abrir navegador, usar red ni modificar datos.

## 4. Evidencia E2E autorizada

- [x] 4.1 Ejecutar una carrera real en ambiente autorizado solo con confirmación explícita de concurrencia, tarea descartable y controles MySQL de lectura; conservar evidencia saneada y validar gate/legacy al cierre.

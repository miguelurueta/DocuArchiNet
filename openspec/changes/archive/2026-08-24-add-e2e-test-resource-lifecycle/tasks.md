## 1. Núcleo de recursos E2E

- [x] 1.1 Crear el registro estricto de contratos de recursos y la validación recursiva de descriptores no sensibles. Verificación: una prueba unitaria acepta un contrato registrado y rechaza claves sensibles, SQL, comandos, proveedores y recursos desconocidos.
- [x] 1.2 Implementar el almacén local de reservas con claves opacas, adquisición atómica, liberación, consumo y detección segura de reserva vigente. Verificación: dos solicitudes locales del mismo recurso dejan exactamente una reserva activa y la evidencia no contiene el identificador de negocio.
- [x] 1.3 Incorporar la huella de generación y el estado de consumo para impedir la reutilización hasta que el adaptador confirme una preparación nueva. Verificación: una transición exitosa bloquea el recurso con la misma generación y una generación distinta lo habilita de nuevo.

## 2. Orquestación y cierre comunes

- [x] 2.1 Integrar preflight registrado y reserva en la secuencia E2E, después de validación y captura efímera pero antes de abrir la etapa mutante. Verificación: preflight no disponible, ambiguo o reservado impide ejecutar el hijo y no abre endpoint mutante.
- [x] 2.2 Finalizar en `finally` todos los recursos adquiridos y agregar evidencia saneada de preflight, reserva, consumo y cierre. Verificación: error, timeout e interrupción liberan o marcan consumo según contrato sin conservar secretos.
- [x] 2.3 Exigir proveedor de reserva compartido para contratos que declaren alcance multi-equipo y fallar cerrado cuando no esté registrado. Verificación: una prueba de contrato rechaza una reserva compartida sin proveedor y permite la local solo en su ámbito declarado.

## 3. Adaptación inicial de Workflow DOC-32

- [x] 3.1 Registrar el adaptador Workflow DOC-32 con los recursos lógicos de ejecución y concurrencia, preflight de solo lectura y condiciones de disponibilidad configuradas en código. Verificación: pruebas focales devuelven códigos saneados para tarea ausente, actividad no disponible, destino sin prerrequisitos y recurso apto.
- [x] 3.2 Migrar el registro y perfil DOC-32 para enlazar los recursos reservados conservando temporalmente sus campos no sensibles actuales. Verificación: perfil de ejemplo válido, tareas distintas y etapas específicas reciben el descriptor correcto sin modificar suites propietarias.
- [x] 3.3 Mantener preview/token/aserciones DOC-32 y propagar el resultado de etapa para marcar consumo solo después de una transición efectiva. Verificación: preview no consume recursos, fallo funcional los libera y éxito registra la generación final.

## 4. Pruebas, documentación y validación

- [x] 4.1 Añadir pruebas unitarias del ciclo de recursos, del adaptador DOC-32 y de los caminos de cierre, sin TTY, navegador ni servicios reales. Verificación: cubren colisión, consumo, nueva generación, preflight rechazado y limpieza de secretos.
- [x] 4.2 Extender las pruebas de política para prohibir detalles sensibles o de negocio en perfiles, marcadores y evidencia de recursos. Verificación: detectan secretos, SQL, comandos e identificadores no saneados sin imprimir sus valores.
- [x] 4.3 Documentar cómo registrar una prueba de otro dominio, escoger alcance de reserva, declarar preflight y preparar una nueva generación de recursos. Verificación: la guía no contiene supuestos de tareas ni actividades Workflow.
- [x] 4.4 Ejecutar las suites locales focales del ciclo de recursos. Verificación: 44 pruebas de contrato, política, orquestación y backend DOC-32 superan sin TTY, navegador ni servicios reales.
- [x] 4.5 Con autorización explícita posterior, ejecutar una E2E DOC-32 sobre recursos preparados. Verificación: la corrida confirma reserva, preflight, transición o bloqueo seguro, consumo y cierre sin tocar gate ni páginas legacy.

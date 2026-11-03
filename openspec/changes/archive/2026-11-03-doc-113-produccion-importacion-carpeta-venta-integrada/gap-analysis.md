# DOC-113 — Reutilización, brechas y evidencia de partida

Fecha: 2026-11-03. Inspección de la base main 1b579739 y pruebas locales sin autenticación. No se modifica código de producto en este corte.

| Área | Ya disponible | Brecha / tratamiento |
|---|---|---|
| Apertura/etapas/destino | Host WebForms, validación servidor, instancia y cuatro etapas; menú conserva carga individual. | Reutilizar; acreditar gesto completo UI-01, sin rehacer ventana. |
| Inventario/revisión | File por identidad, raíces homónimas, rutas normalizadas, selección repetida, paginación e invalidación de respuestas viejas. | Reutilizar; UI-02 debe ejercitar selector real y cambio de inventario, no solo driver REC. |
| Transporte | API propia existente con sobre, timeout, aborto y cola sin replay. | No crear otro HTTP client; completar validación de datos en el límite de consumo de la pantalla. |
| Respuesta de preparación | Se valida que Incidencias sea array. | F-01 reproducido: un elemento null pasa y rompe render; corregir antes de asignar modelo. |
| Ciclo de vida | mountedHost destruye una instancia sustituida y Sys.Application.add_load vuelve a inicializar; foco/escape y cierre ya existen. | Falta caso explícito de reemplazo/desaparición y actualización parcial en la suite cliente; probar antes de proponer hooks o cambios de comportamiento. |
| E2E | Sesión, consola, DSN/SELECT, perfiles y reporteros compartidos; REC/reapertura con límites documentados. | UI-01/02/03 no están registrados como recorrido completo en el DOM real; añadir especificación focal y selector, sin otro login/runner/plataforma. |

## Línea base ejecutada

`node --test tests/doc111-importacion-carpetas-client.test.cjs`: 13 pruebas, 13 aprobadas, 0 fallidas, 0 omitidas. Usa Chromium instalado en tools/e2e con servicios sintéticos; no es una aceptación IIS, MySQL, selector real del sistema o efectos documentales. Los seis ficheros comprobados coinciden con el worktree DOC-113. Registro local: .tmp/doc113-client-baseline.tap.

## F-01 — Preparación con incidencias no consumibles

La sonda .tmp/doc113-probe-response.cjs cargó los assets de la base y respondió AbrirImportacion/PrepararPlan en memoria. Con resultado.Incidencias=[null] se observó `Cannot read properties of null (reading 'RaizIdCliente')`; quedó un diálogo, busy=false, mensaje vacío y cero botones Confirmar. Hubo cero peticiones HTTP externas, cero login y ningún efecto de servidor. Registro .tmp/doc113-response-probe.json. Esa reproducción prueba el defecto actual; no acredita un arreglo ni es una nueva suite de aceptación.

## Archivos propuestos y reutilización

La suite `tools/e2e/tests/doc113-ventana-importacion.spec.cjs`, su soporte focal `tools/e2e/tests/support/doc113-ventana-importacion.cjs` y el selector `tools/e2e/scripts/run-doc113-ventana-importacion.cjs` todavía no existen. El soporte consume openSession de DOC-111, que ya delega al helper authenticated-workflow-session; la consola usa interactive-e2e-console. El perfil base mantiene su validación cerrada; referencias UI alternativas se pasan como parámetros no secretos validados. No se crea .env, configuración Playwright, autenticación o permisos paralelos.

## Cierre y restricciones

La autorización histórica DOC-111 no se traslada a estas corridas. UI-02 puede persistir planes/confirmación y debe declarar los recursos descartables y efectos antes de ejecutarse. No se recibe ni almacena contenido en el recorrido UI. Toda captura/traza debe sanearse y distinguir datos reales de fixtures; nunca retener secretos. La matriz futura registra UI-01/02/03 como EJECUTADO_OK, FALLIDO o NO_EJECUTADO con SHA/huella y responsable. MSBuild Framework y validación real todavía no se ejecutaron por este refinamiento.

## Correspondencia con el plan inicial

Las tareas iniciales 1.1/1.2 quedan sustituidas por este refinamiento y la trazabilidad; 2.1 se concreta en 2.1 y 4.1–4.4; 2.2 en 3.1 y la conservación del recorrido existente; 3.1/3.2 en 2.2, 5.1, 6.1 y 6.2; 4.1/4.2 en 1.1 y 6.3. Se mantienen todas las obligaciones del ticket, sin contabilizar componentes resueltos como desarrollo nuevo.

## Resultado local posterior

F-01 queda cubierto por pruebas de incidencias/elementos/valores nulos y perfiles inválidos: error visible, sin pageerror ni confirmación. Se encontró y corrigió F-02 de retorno de foco en el mismo controlador. Suite cliente ampliada 15/15 PASS; driver/política 43/43 PASS. Registro/discovery de tres casos PASS; ejecutar la suite sin autorización omite 3/3, sin login ni peticiones del sistema. Esto no acredita los tres casos reales. MSBuild Framework de Visual Studio completó Build con salida aislada, sin despliegue.

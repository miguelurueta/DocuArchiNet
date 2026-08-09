# Plan de migración: manejo de sesión Workflow

## Objetivo

Migrar la detección de sesión expirada e indisponibilidad temporal de Workflow sin cambiar los flujos, tareas ni handlers existentes.

## Archivos del paquete

```text
js\workflow\session-workflow.js
Defaul\WebFormInicioDocuarchiGestion.aspx
workflow\Webworkflow.aspx
workflow\WebPrincipal.aspx
workflow\WebFormGestionFlujoTrabajoCamaras.aspx
```

## Dependencia confirmada

`workflow\refresh_session.ashx` y su code-behind ya existen en el repositorio local. El code-behind debe validar la sesión de usuario de Workflow; no debe usar variables temporales de carga de archivos como indicador de sesión.

## Implementación

1. Actualizar `workflow\refresh_session.ashx.vb` para devolver JSON `{"active":true}` con HTTP 200 únicamente si la identidad autenticada y alguno de los identificadores de usuario de sesión son válidos. Debe devolver HTTP 401 y JSON `{"active":false}` cuando la sesión no existe, con `SuppressFormsAuthenticationRedirect` y `TrySkipIisCustomErrors` activos.
2. Copiar `js\workflow\session-workflow.js` desde `D:\temfile\Gestion` al mismo path del repositorio y hacer que el sondeo redirija también si el handler devuelve HTTP 401.
3. Confirmar que jQuery se carga antes del nuevo script en cada página.
4. Agregar la referencia del script después de jQuery en `Defaul\WebFormInicioDocuarchiGestion.aspx`:

```html
<script src="../js/workflow/session-workflow.js?v=20260808-sessionfix6" type="text/javascript"></script>
```

La página principal carga los módulos en `ContentPlacenter_ifrm_ds_`; esta única instancia cubre todas las páginas abiertas desde el menú. No cargar el monitor en los iframes de Workflow: evita sondeos duplicados y redirecciones silenciosas en competencia.

5. Retirar de `WebPrincipal.aspx` el temporizador heredado `MantenSesion`; el sondeo centralizado ya mantiene y valida la sesión.
6. Reutilizar el popup central `#modal_alert_sesion_time_out` para toda sesión finalizada; no usar alertas JavaScript nativas. El botón existente `Button_rdedirect_pag` conserva la redirección a `gestor.aspx`.
7. No tocar funciones de tareas, `UpdatePanel` ni enlaces de navegación.

## Comportamiento esperado

- HTTP 401/403 o respuesta de login: se muestra el popup central con fondo estático; al aceptar, se realiza una sola redirección a `gestor.aspx`.
- HTTP 0/503: manejo de interrupción y sondeo de recuperación, sin bucles de redirección.
- La redirección no se ejecuta más de una vez por sesión interrumpida.
- Sin errores de red, el cliente consulta `refresh_session.ashx` cada 60 segundos para detectar una sesión eliminada o vencida.
- Los manejadores se conservan después de postbacks parciales.

## Validación

- Sesión válida: no hay alertas ni redirecciones.
- Sesión expirada: redirección única al login.
- Reinicio temporal del servidor: se muestra el mensaje previsto y no se multiplica el sondeo.
- Abrir módulos de Workflow, Gestión y Docuarchi desde el menú principal: debe existir una sola solicitud periódica de `refresh_session.ashx` en la ventana contenedora, sin errores de consola ni temporizadores de sesión en los iframes.
- Compilar y publicar el proyecto: la directiva `CodeBehind` requiere que `refresh_session.ashx.vb` sea incorporado a la DLL desplegada antes de validar el endpoint en IIS.

## Reversión

- Revertir `workflow\refresh_session.ashx.vb`, restaurar el temporizador heredado solo si fuera necesario y retirar la referencia de `session-workflow.js` de la página contenedora.
- Eliminar o revertir solo `js\workflow\session-workflow.js`.
- No alterar funciones de negocio ni rutas de navegación.

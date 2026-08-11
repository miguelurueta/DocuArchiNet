# Plan de migración: menú responsivo y cabecera de Inicio

**Estado:** implementado en el repositorio local el 2026-08-07. Incluye la capa de paridad visual con la versión vigente de IIS; pendiente de validación en ejecución local antes de publicar un nuevo precompilado.

## Objetivo

Llevar al repositorio local la mejora visual y responsiva del menú lateral y la cabecera de `Defaul/WebFormInicioDocuarchiGestion.aspx`, conservando IDs, eventos, conteos y funciones existentes.

## Archivos del paquete

```text
Defaul\WebFormInicioDocuarchiGestion.aspx
Styles\menu-vertical-responsivo.css
Styles\inicio-menu-iis.css
js\inicio\menu-vertical-responsivo.js
```

## Alcance confirmado

- Menú lateral se contrae en pantallas reducidas y se controla con el botón existente `.sidebar_toggle`.
- El botón debe responder en toda su superficie, no solamente sobre el icono.
- Los hijos de `.scoop-right-header` se mantienen en una única fila.
- Los elementos con `.d-none` continúan ocultos.
- Los botones visibles de cabecera se compactan sin modificar sus `onclick`.
- El usuario se muestra con módulo y login usando los datos ya emitidos por la página, sin borde decorativo. El módulo se conserva en `title` y el login en `#user_usuario_loguin`; no se reemplaza el HTML del control.
- El área de marca (`#hader_logo`) usa exactamente el mismo fondo oscuro que la barra lateral (`#0b1930`).
- Se eliminan separadores visuales aprobados en los grupos de tarjetas de Inicio.

## Implementación

1. Comparar cada archivo destino con su equivalente en `D:\temfile\Gestion`.
2. Copiar `Styles\menu-vertical-responsivo.css` y `js\inicio\menu-vertical-responsivo.js` al repositorio.
3. Mantener `Styles\inicio-menu-iis.css` como capa posterior de paridad visual: concentra la presentación aprobada de menú, cabecera y tarjetas sin introducir lógica de sesión.
3. En `WebFormInicioDocuarchiGestion.aspx`, cargar los recursos después de los recursos base del menú:

```html
<link href="../Styles/menu-vertical-responsivo.css?v=20260807-menu11" rel="stylesheet" />
<script src="../js/inicio/menu-vertical-responsivo.js?v=20260807-menu2"></script>
```

4. Cargar después del bloque visual histórico de la página:

```html
<link href="../Styles/inicio-menu-iis.css?v=20260807-home-menu1" rel="stylesheet" />
```

5. Si cambia el contenido durante la migración, aumentar solo el parámetro `v` del recurso afectado.
6. No sustituir la página completa: trasladar únicamente las referencias, la cabecera y los bloques visuales identificados.

## Validación

- Ancho normal: menú expandido, botones en una sola fila.
- Ancho reducido: menú se contrae automáticamente.
- Clic en cualquier punto de `.sidebar_toggle`: alterna el menú.
- Inicio, descarga, usuario y cierre de sesión conservan sus eventos.
- Elementos con `.d-none` no aparecen.
- No se generan errores JavaScript en consola después de una actualización parcial.

## Reversión

- Retirar las dos referencias añadidas de la página.
- Restaurar exclusivamente los dos archivos CSS/JS de este paquete desde el commit o respaldo previo.
- No modificar estilos globales de Bootstrap o del componente Scoop para revertir.

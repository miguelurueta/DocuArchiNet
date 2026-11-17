# DOC-120 — diagnóstico visible de parada por archivo

Corrección solicitada por el usuario después del video PARADA: el aviso no identificaba claramente motivo, archivo ni avance. El análisis del video observa lote50, perfil Egresosv3,1.000 PDF recibidos, expediente creado y primer documento confirmado al consultar. El siguiente archivo mostrado es1098.pdf/98; su resultado final no se deduce del video y no se ejecutó una recuperación real sobre él.

## Defectos reproducidos y cambios

- El transporte conserva TIEMPO_AGOTADO y30 segundos predeterminados, pero describirError reemplazaba la causa por «No se confirmó el resultado». Ahora conserva el motivo público y el tiempo configurado del cliente; no cambia el timeout. Una respuesta desconocida declara que no permite identificar la causa, sin inventar fallo de PDF/base de datos.
- La coordinación marcaba bloqueo internamente, pero esperaba la consulta posterior antes de publicar ese estado. Se reprodujo último estado visual running con estado interno blocked. Ahora publica inmediatamente blocked/consultandoResultado y muestra la consulta separada del guardado. La consulta existente sigue siendo única, sin polling ni replay; cierre invalida respuestas tardías.
- El panel identifica fase, nombre, ruta relativa y posición dentro del inventario completo cuando se conoce; conserva el último avance confirmado y distingue ese conteo de la posición del archivo. Incluye código/referencias en Datos para soporte. La consulta pendiente bloquea acciones duplicadas y no oculta el aviso amarillo.
- Si la consulta posterior confirma el documento, reemplaza la incertidumbre por guardado confirmado y mantiene pausa; si observa intento activo, muestra ese estado e intento. Si también falla la consulta, conserva el motivo inicial y añade el fallo de lectura. Ninguno de esos resultados autoriza repetir el almacenamiento.

Los cambios se limitan al cliente, estilos y versiones de recursos en el ASPX. No alteran contratos ASMX/ASHX, esquema, perfiles, reglas de integridad, escritores documentales, SII o cuentas. La lentitud del servidor requiere medición propia; este corte mejora diagnóstico y transición visual.

## Verificación

Pruebas locales con módulos reales, Chromium y transporte sintético: timeout30s/45s, catálogo público en las cinco fases, privacidad/denegación, respuesta desconocida, acuse confirmado seguido de fallo de lectura, parada del segundo archivo después del primero confirmado, consulta demorada/fallida/activa, confirmación observada sin continuar sola y nombres largos como texto en390/768/1280px. Se verifica un único intento por recurso y cero efectos de red reales en los fixtures. Log, resultado, SHA de fuentes y capturas en el JSON compañero.

Lectura técnica del localhost:4 GET estáticos200; JavaScript/CSS servidos coinciden byte a byte con la carpeta principal.0 sesiones autenticadas/SELECT/efectos de negocio. Sin reemplazar DLL ni reiniciar el pool. La siguiente apertura con recursos `doc120-parada-20261117-1` carga el aviso actualizado; una ventana ya abierta conserva su JavaScript hasta recargar.

## Operación

Ante una parada, conservar el lote y el recurso indicado. Consultar resultado actualiza la lectura, sin recibir PDF ni repetir almacenamiento. Si existe una confirmación, continuar solo con las acciones disponibles; si queda intento activo o incierto, mantenerlo para la recuperación/diagnóstico permitido por el servidor. Una espera agotada no demuestra que el documento no se guardó.

Tareas3.132–3.134 verificadas localmente; este resultado no cierra la aceptación de capacidad real ni las tareas anteriores pendientes. El ajuste de BENEFICIARIO sigue con su evidencia separada de precondición fallida; el video muestra otra versión de Egresos y no acredita que las corridas de ese ajuste hayan escrito cambios.

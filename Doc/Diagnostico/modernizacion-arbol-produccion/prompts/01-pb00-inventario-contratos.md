# Prompt PB00 para inventariar los contratos del explorador

**Estado:** preparado para revisión y ejecución de inventario documental. No implementa aplicación ni aprueba decisiones.

## Objetivo

Actúa como arquitecto de ASP.NET WebForms y JavaScript. Lee [el contexto común](contexto-comun.md) y caracteriza los contratos del árbol actual para que cada cambio posterior tenga consumidores, comportamiento y verificaciones concretos. El resultado debe permitir implementar sin deducir reglas a partir de los colores o de los datos simulados de la maqueta.

## Insumos y archivos a revisar

Lee la exploración y revisa, por nombre de función, estos archivos desde la raíz:

- `Gestion/WebFormProducionDocumental.aspx` y su `.aspx.vb`.
- `Gestion/Class_niveles_organizacion.vb` y `Gestion/Class_ra_pro_niveles_has_expediente_archivo.vb`.
- `Gestion/ClassGaProducionDocumental.vb` y `Gestion/ClassGaExpediente.vb` para las operaciones invocadas.
- `webservice/WebServiceProducion.asmx.vb` y los servicios de permisos/archivos realmente alcanzados.
- `js/gestion/WebFormProducionDocumental.js` y `js/jquery.contextMenu.js`.
- Integración `Doc119NivelPulsado`, `Doc111ActualizarRama`, `Doc120MostrarExpediente` y separación de formularios de Excel.

## Trabajo requerido

1. Documenta el recorrido exacto desde clic, teclado, búsqueda y menú hasta campos ocultos, postback/ASMX, sesión, validación y respuesta.
2. Inventaría cada operación del diagnóstico. Por fila registra entrada visual, handler/servicio, tipo aplicable, identidad requerida, permisos efectivos, variables de sesión leídas/escritas, efecto de dominio y formato de respuesta. Distingue hechos por código de comprobaciones de ambiente pendientes.
3. Caracteriza al menos el nivel vacío/propio/recibido/compartido y el expediente con identidad compuesta. Incluye nodos iniciales y dinámicos y enlaces de imagen/texto para un mismo nodo.
4. Verifica dónde se toma la primera relación del expediente, dónde se cambia la relación al mover y qué elimina exactamente la operación de dominio. No propongas una regla de unicidad sin evidencia.
5. Revisa cómo se confirma la selección y qué ocurre al limpiar, fallar el postback, usar clic derecho sobre otro nodo o compartir sesión entre pestañas. Identifica qué rutas requieren cambios en PB01/PB02 y cuáles solo necesitan presentación.
6. Registra una lista finita de consumidores a preservar: búsqueda, expansión, creación/edición, traslado, ocultos, importación, archivo físico, rótulos, estados, índices, carga, archivos y acciones múltiples. Señala cualquier ruta que no se haya podido trazar.
7. Propón los contratos mínimos para los paquetes siguientes sin implementarlos: datos de identidad, autoridad, correlación y errores. Relaciónalos con decisiones pendientes, sin elegir firmas definitivas ni endpoints por anticipado.

## Verificación

Comprueba las suites locales existentes antes de invocarlas y ejecuta las que sean independientes del ambiente para fijar la línea base. Como candidatas:

```text
node --test tests/doc119-context-menu.test.cjs tests/doc120-importacion-arbol.test.cjs
node --test tests/doc126-importacion-excel-ui.test.cjs
```

No ejecutes pruebas autenticadas ni carga. Un fallo de la línea base se registra con su causa conocida y efecto sobre la implementación; no se corrige aplicación dentro de este inventario.

## Criterios de aceptación

- Cada familia del inventario tiene entradas, validaciones, resultados y consumidores localizados.
- Se conoce qué necesita backend y qué se resuelve en cliente.
- Las dudas de ambiente tienen dato requerido y comprobación propuesta, no una respuesta inventada.
- El contrato propuesto identifica las decisiones que lo habilitan y cualquier incompatibilidad con las alternativas.
- No se modifica código, base de datos, decisiones ni el cambio OpenSpec DOC-127 como consecuencia del inventario.

## Entrega

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB00-contratos-vigentes.md` con el mapa de operaciones, evidencia por función, línea base, riesgos y dependencias. Actualiza los vínculos documentales pertinentes y registra únicamente estos cambios de documentación. El paquete termina con una lista concreta de entradas para PF01 y PB01, no con una declaración de implementación realizada.

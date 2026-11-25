# DOC-126: corrección del guardado mostrado en video

Corte 2026-11-24T22:45:45.749Z; base 73df802cf95c3f893a69ce2854b7ced90de867f4. Tarea 8.9 completada, 48/53 en total. Permanecen abiertas 6.9, 6.10, 6.11, 7.5, 8.8.

La grabación muestra que RELACIÓN ORDENES 2025.xlsx se analiza y la interfaz informa 12759 filas. Se selecciona NROORDEN, pero Tabla de destino conserva sólo imp_02_. El cliente rechaza ese nombre antes de enviar GuardarPlantilla y el mensaje anterior no indicaba el campo. El video no acredita un guardado ni importación terminados. No se copiarán capturas con contenido de las celdas al repositorio.

Ahora se propone imp_02_relacion_ordenes_2025 al analizar ese nombre de archivo para crear. El nombre es editable, conserva nombres manuales y se actualiza si cambia el archivo y aún se usa la sugerencia. Agregar/reemplazar conservan su destino. Se normalizan acentos y separadores, con máximo de 64 caracteres. No se comprueba ni evita silenciosamente una colisión creando otro destino; guardar mantiene las validaciones actuales del servidor.

Una tabla incompleta o clave que no existe en los destinos se señala con mensaje específico, aria-invalid, descripción y foco, sin enviar el guardado. Las marcas se retiran al corregir. El botón Quitar conserva una línea. No cambia la confirmación, el consentimiento, los contratos, la lectura, los límites, el esquema o el contenido del archivo. Guardar sigue siendo explícito y no importa.

Verificación: 55/55 pruebas existentes de browser/contrato/scopes/navegación, incluidas tres reproducciones nuevas del problema, nombre manual/cambio de archivo/longitud, clave inválida, foco y botón Quitar. Transporte simulado y libros sintéticos: no es E2E autenticada. IIS sirve JS/CSS exactos con doc126-3, contrastados por hash mediante GET anónimos. No se ejecutó SQL ni se inició sesión. No se compiló ni reemplazó la DLL en esta corrección; se conserva la encontrada en ejecución, hash observado 09a5bffa9ba1f4a6b6a44b42c226b73ba888ca880b44e1fc26adc57e8b1a91d3. El hash difiere del despliegue documentado anterior: no se sobrescribe esa variación local.

Los assets y ASPX están en la carpeta principal servida por IIS. Recargar con Ctrl+F5 para renovar recursos. No requiere nuevos SQL. El scope local de la E2E debe refrescar sus hashes antes de cualquier ventana autorizada; las cuentas/IDs, instalación aplicable y autorización específica continúan pendientes. La observación del video y esta corrección no cierran 6.9/6.10/6.11/7.5/8.8.

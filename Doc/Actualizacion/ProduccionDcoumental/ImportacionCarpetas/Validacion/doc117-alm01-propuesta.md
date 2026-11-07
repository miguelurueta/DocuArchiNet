# DOC-117 — Propuesta técnica, sin ejecución real

Diseño aprobado editorialmente. Esta propuesta no autoriza cargas, credenciales ni instalación de DLL. Conservar todos los recursos; no tocar lote 18/intento 41 ni repetir efectos inciertos. Controles SELECT por los DSN registrados MembershipUsers, sin URL MySQL. Usuario de aplicación y cuenta MySQL de lectura son identidades distintas. Secretos efímeros exclusivamente en consola.

## Alcance y presupuesto máximo propuesto

Dos preparaciones y dos confirmaciones, una por versión 40 (perfil 26, formato A) y 42 (perfil 27, formato B), nivel 7/gabinete 10. Una raíz y un PDF de prueba de 677 bytes por plan: hasta dos lotes, dos recepciones, dos creaciones y dos documentos definitivos. Hasta dos repeticiones idénticas de almacenamiento después de comprobar confirmación durable. No crear perfiles ni versiones. No usar versión 41 para almacenar.

## Recursos y valores

El manifiesto exacto está propuesto en doc117-alm01-manifiesto-20261105.md y tools/e2e/fixtures/doc117-alm01-manifest.json; requiere comprobar vigencia y ausencia por lectura antes de cualquier efecto. Formato A usa el patrón de DOC114-79.pdf y raíz codigo-fecha; B usa 79.pdf y raíz codigo#fecha. Los ejemplos históricos 000114-20261103 / 000114#03-11-2026 no se reutilizan como nuevas raíces. Proponer dos códigos nuevos distintos con ceros a la izquierda, comprobar inexistencia por SELECT y conservar la fecha equivalente en ambos formatos. No asumir que tipología 79 sigue vigente: revalidar referencia/código, clasificación y relación con gabinete.

Antes de autorizar ejecución, el manifiesto debe mostrar los nombres exactos y todos los valores esperados de expediente e índices, salida del motor actual, hash de snapshot y firma de los PDF. Si el sufijo no satisface reglas, detener: no modificar perfiles para sortearlas. La receta de raíz única ya está implementada y probada en el driver DOC-117; no invoca runRec completo.

## Verificación

Comparar valores planificados/fijados y finales: ceros de TEXTO/REFERENCIA, fecha canónica, tipología, nombre/extensión, gabinete, documento/inventario, expediente e incorporación por IDs conocidos. DECIMAL solo si está en las reglas efectivas; no inventar un campo nuevo para este caso. Verificar originales por bytes/hash y conservar staging. Los casos no presentes en estas reglas mantienen su evidencia local explícita. Si aparece incertidumbre, consultar estado antes de otra solicitud y no volver a almacenar.

## Estado

NO_EJECUTADO. Presupuesto propuesto, no reservado ni consumido. Manifiesto exacto validado offline; driver local preparado; faltan preflight SELECT real y autorización específica de los efectos.

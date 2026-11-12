# DOC-119 — recuperación real verificada con límites

Corrida29623218ea64414d96d14937cb5ffebc, commit74083c36eb4634ccc217cb2071d962c68a29d49d, localhost/GestionDocumental-Docuarchi.net, lote29/plan45, propietario141/empresa2, destino original7 y seleccionado40. Final2026-11-12T21:00:07.726Z, **FINISHED_WITH_LIMITS**, sin fallo, cerrada y credenciales eliminadas. [Evidencia saneada](doc119-recuperacion-verificada-20261112.json), verificada contra el estado y los hashes de los nueve archivos de código registrados.

SEG-02 **PASS_READONLY_WITH_LIMITS**: recarga, recuperación de referencia, reselección explícita del PDF pendiente sin envío, consulta posterior, rama original disponible y una sola relación lógica tanto antes como después. Selección y expansión permanecen iguales. No se repitieron preparación, confirmación, recepción, creación ni SEG-01; no se pidió Carolina.

Consumo **281HTTP/27SELECT/1sesión**, dentro de600/60/2. Huellas de negocio antes/después idénticas: b6c564f13b168822728f35e09ec7f551dde0b1da9ad183c68d65223bf108b278. Alcance DA: expedientes conocidos del lote, no inventario global. Permanecen27 archivos,26 pendientes, uno recibido de677 bytes, un expediente y cero documentos almacenados. La recepción no se anuncia como almacenamiento.

El diagnóstico del árbol reporta AVAILABLE y conteos lógicos[1]→[1], con selección/expansión conservadas. Esto verifica la deduplicación de la relación existente y la corrección de la confusión entre enlaces de imagen/texto en el recorrido real. No acredita inserción de una relación nueva: ya estaba representada. Los nombres físicos de las dos raíces son distintos, por lo que tampoco acredita reselección entre raíces físicas homónimas.

La inserción por IDs ausentes y los homónimos están cubiertos por los recorridos locales de Chromium/cliente real con servicios sintéticos documentados en `doc119-recuperacion-arbol-20261112.md`, incluidos HTML WebForms imagen/texto y rechazo de duplicados lógicos reales. Suite cliente28/28 y políticas49/49 corresponden al commit de esta corrida; no se ejecutaron de nuevo por esta actualización documental ni se suman como pruebas reales.

Progreso **7/24** hasta resolver la aceptación de los dos casos no observados en IIS. La tarea2.5 y la compuerta2.7 permanecen abiertas. [Decisión preparada de cobertura](doc119-aceptacion-cobertura-mixta-propuesta-20261112.md). No se habilita UI por inferir aprobación de un resultado con límites.

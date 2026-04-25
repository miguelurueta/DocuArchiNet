# Prompt: 23-FE AppEditor Fase 1 paginas reales base

Actua como arquitecto y desarrollador senior especializado en TipTap, ProseMirror, motores de documentos paginados y editores tipo Word.

Necesito disenar e implementar la `AppEditor Fase 1 - paginas reales base`.

=====================================================================
OBJETIVO

Reemplazar la base actual de paginacion visual de `AppEditor` por una arquitectura de paginas reales.

El sistema actual no debe seguir dependiendo de:

- `pageBreak` automaticos como mecanismo principal
- `spacerHeight`
- simulacion visual de hojas
- correccion posterior al desborde

La nueva base debe trabajar con hojas reales en el modelo del editor.

=====================================================================
CONTEXTO

Actualmente `AppEditor`:

- usa flujo continuo editable
- representa hojas visualmente
- inserta `pageBreak` automaticos
- recalcula cortes mediante medicion DOM y espaciadores

Esto no garantiza margen real ni hojas reales.

=====================================================================
RESULTADO ESPERADO DE ESTA FASE

Implementar solo la base estructural:

- nodo `page` real
- `doc` compuesto por paginas
- area util real por pagina
- continuidad basica a pagina siguiente al llegar al final
- compatibilidad con TipTap/ProseMirror
- aislamiento o retiro del mecanismo viejo como base primaria

IMPORTANTE:
Esta fase NO debe intentar cerrar todavia toda la complejidad de reflow fino, merge inverso ni hardening completo. Eso ira en Fase 2.

=====================================================================
RESTRICCIONES

NO hacer:

- parches CSS para esconder overflow
- mantener dos motores compitiendo como base del editor
- seguir usando `pageBreak + spacer` como solucion principal
- re-render completo del editor en cada cambio

SI hacer:

- rediseño estructural del schema
- modelo de paginas reales
- ruta de migracion del contenido existente

=====================================================================
ALCANCE OBLIGATORIO

1. Definir nuevo schema de pagina real.
2. Ajustar `doc` para contener nodos `page`.
3. Definir que bloques viven dentro de una pagina.
4. Implementar render y parse coherente.
5. Implementar creacion de nueva pagina real al llegar al limite inferior.
6. Mantener:
   - toolbar
   - serializacion
   - modo controlado/no controlado
   - imagenes locales
   - links
   - zoom
7. Identificar y apagar o retirar la base de paginacion vieja donde corresponda.

=====================================================================
ARCHIVOS A REVISAR COMO MINIMO

- `presentation/AppEditor.tsx`
- `application/useAppEditor.ts`
- `application/normalizeEditorHtml.ts`
- `infrastructure/tiptap.config.ts`
- `infrastructure/tiptap.extensions.ts`
- `application/autoPagination.ts`
- `application/autoPageBreak.ts`
- cualquier nuevo archivo necesario para `page`

=====================================================================
ENTREGABLES

1. Diagnostico tecnico corto de que piezas viejas dejan de ser base del sistema.
2. Implementacion de paginas reales.
3. Estrategia de migracion desde documentos existentes.
4. Ajuste de tests minimos para validar la nueva base.

=====================================================================
CRITERIOS DE ACEPTACION

- existen paginas reales en el modelo
- el contenido no se monta sobre el borde inferior
- al terminar una pagina, la continuidad va a la siguiente
- no depende de espaciadores visuales viejos
- no se rompe el editor en casos basicos de escritura y apertura

=====================================================================
SALIDA ESPERADA

Quiero cambios reales en codigo, no solo propuesta.

Entrega:

- resumen tecnico corto
- archivos tocados
- riesgos residuales
- pruebas ejecutadas o no ejecutadas

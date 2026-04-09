# PROMPT ARQUITECTONICO  Ticket 03 FE
# UI/UX AppUpload: preview, acciones y responsive

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Implementar la UI de AppUpload con previews tipo galeria, acciones visibles por hover y layout responsive grid/list segun la arquitectura.


CONTEXTO EXISTENTE

- especificaciones UI/UX: `docs/Architecture/AppUpload/AppUpload-Architecture.md`


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppUpload/
```


RESTRICCIONES (OBLIGATORIAS)

- no introducir estilos globales
- mantener consistencia visual con `AppInput`


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. PREVIEW
   - renderizar preview visual por archivo (thumbUrl/url)
   - fallback por tipo de archivo

2. ACCIONES
   - hover: acciones visibles
   - click: preview opcional y configurable
   - remove: boton overlay (X)
   - soporte teclado: Enter (preview) y Delete (remove)

3. RESPONSIVE
   - Desktop: 46 columnas
   - Tablet: 23 columnas
   - Mobile: 2 columnas
   - padding reducido en mobile
   - imagenes mas compactas en mobile

4. BOTON DE CARGA
   - visible solo si no se alcanza el limite

5. DRAG & DROP
   - soporte `drag?: boolean`
   - estados visuales para hover valido / invalido

6. CARDS VISUALES
   - bordes suaves
   - hover elevation
   - `aspect-ratio: 1/1`

7. ACCESIBILIDAD
   - `aria-label` en acciones principales
   - focus visible en items y acciones


PRUEBAS UNITARIAS (OBLIGATORIAS)

- render de previews con y sin imagen
- remove overlay visible en hover
- boton de carga se oculta con maxCount
- drag & drop aplica estado valido/invalido
- focus visible y aria-label presentes


PRUEBAS QT (CALIDAD / E2E)

- hover muestra acciones
- click abre preview (si habilitado)
- responsive correcto por breakpoint
- drag & drop funciona con estado visual


CRITERIOS DE ACEPTACION

- UI consistente y reutilizable
- layout adaptativo en grid/list
- interacciones principales funcionales

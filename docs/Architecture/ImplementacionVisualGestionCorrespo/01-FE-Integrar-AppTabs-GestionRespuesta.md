# PROMPT ARQUITECTONICO  Ticket 01 FE
# Integrar AppTabs en GestionRespuesta (layout principal)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Reemplazar el contenido actual de `GestionRespuesta.tsx` por un layout basado en `AppTabs`, manteniendo el `AppButton` de "Volver a la bandeja" fuera del contenido de tabs y preservando la arquitectura visual existente del modulo `gestionCorrespondencia`.


CONTEXTO EXISTENTE

- arquitectura de referencia: `docs/Architecture/ImplementacionVisualGestionCorrespo/GestionRespuesta-AppTabs-Architecture.md`
- contrato de `AppTabs`: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- vista objetivo: `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
```


RESTRICCIONES (OBLIGATORIAS)

- no modificar el componente `AppTabs`
- no introducir logica de negocio nueva
- no mover el `AppButton` dentro de los `children` de un tab
- no crear estilos globales
- no romper routing ni handlers existentes


CONTRATO (OBLIGATORIO)

- `AppTabs` recibe `items: AppTabItem[]`
- `AppButton` de "Volver a la bandeja" permanece visible al cambiar tabs


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. El `AppButton` debe estar fuera de `AppTabs`
2. `AppTabs` ocupa el area principal del contenido
3. El contenido actual de `GestionRespuesta` se divide en secciones/tabs coherentes
4. Mantener accesibilidad y foco visible


RIESGOS A EVITAR (OBLIGATORIO)

- esconder el boton al cambiar de tab
- mezclar controlado/no controlado en `AppTabs`
- romper estilos existentes de `gestionCorrespondencia`


PRUEBAS UNITARIAS (OBLIGATORIAS)

- renderiza `AppTabs` en `GestionRespuesta`
- el boton "Volver a la bandeja" permanece visible al cambiar de tab


PRUEBAS QT (CALIDAD / E2E)

- navegar entre tabs mantiene el boton visible
- layout consistente en desktop y mobile


CRITERIOS DE ACEPTACION

- `GestionRespuesta.tsx` usa `AppTabs` como layout principal
- `AppButton` de volver permanece fuera de tabs
- no se rompen rutas ni handlers existentes

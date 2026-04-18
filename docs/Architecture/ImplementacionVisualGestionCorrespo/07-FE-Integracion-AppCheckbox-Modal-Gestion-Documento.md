# PROMPT ARQUITECTONICO
# Ticket FE
# Integracion de `AppCheckbox` en Modal de Gestion de Correspondencia

Rol esperado:
Arquitecto de software senior frontend (React, TypeScript estricto, Design
System enterprise, integracion de shared UI, accesibilidad y composicion de
formularios modales)


## OBJETIVO

Integrar la familia shared `AppCheckbox` dentro del modal
`GestionDocumentoModal`, reemplazando los `AppInput` con `type="checkbox"` por
`AppCheckbox`, manteniendo la misma interfaz funcional del modal y alineando la
UI con el Design System ya implementado para checkboxes.


## CONTEXTO EXISTENTE

- modulo consumidor real:
  `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/modalGestionDocumento/`
- componente actual:
  `GestionDocumentoModal.tsx`
- estilos actuales:
  `GestionDocumentoModal.module.css`
- shell del modal:
  `AppModal`
- otros shared ya presentes en el modal:
  - `AppInputSelect`
  - `AppInputTags`
  - `AppButton`
- nuevo shared disponible para integrar:
  - `AppCheckbox`
  - `AppCheckboxGroup`
  - `AppCheckboxCheckAll`

La UI actual del modal ya tiene:

- titulo `Confirmar envio de respuesta`
- bloque de checks superior
- dos `AppInputSelect`
- `infoBox` con metadata
- `AppInputTags`
- acciones `Cancelar` y `Confirmar envio`


## ALCANCE

- reemplazar el bloque superior de checks por `AppCheckbox`
- mantener el estado local actual del modal
- conservar labels, jerarquia visual y layout general
- mejorar consistencia visual entre checkbox, selects y tags
- mantener accesibilidad y navegacion por teclado

No incluye:

- backend
- submit real
- logica de negocio
- validaciones de dominio
- refactor del layout general del modal fuera del bloque de checks


## UBICACION (OBLIGATORIA)

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/modalGestionDocumento/
```

Archivos a tocar esperados:

- `GestionDocumentoModal.tsx`
- `GestionDocumentoModal.module.css`

Shared consumido:

```txt
src/app/Components/UI/AppCheckbox/
```


## DECISION ARQUITECTONICA

La integracion correcta en esta interfaz es usar `AppCheckbox` individual en el
bloque superior, no `AppInput type="checkbox"`.

Motivos:

- `AppCheckbox` ya concentra el contrato visual y accesible correcto
- evita seguir manteniendo dos patrones distintos de checkbox en la UI
- alinea el modal con la familia reusable creada en `SCRUMCORE-124/125/126`
- simplifica futura evolucion a grupo o `check all` si el flujo lo requiere

En este modal, la opcion correcta inicial es:

- `AppCheckbox` individual para cada decision booleana

No es obligatorio usar `AppCheckboxGroup` ni `AppCheckboxCheckAll` si el flujo
visual actual no lo necesita.


## ESTRUCTURA OBJETIVO

El bloque superior debe migrar de:

```tsx
<AppInput type="checkbox" ... />
```

a:

```tsx
<div className={styles.checksGroup}>
  <AppCheckbox
    label="Solicita al centro de envio de correspondencia el envio de la respuesta"
    checked={solicitaCentroEnvio}
    onChange={(checked) => setSolicitaCentroEnvio(checked)}
    size="md"
  />

  <AppCheckbox
    label="Confirma respuesta al correo electronico del peticionario"
    checked={confirmaCorreoPeticionario}
    onChange={(checked) => setConfirmaCorreoPeticionario(checked)}
    size="md"
  />

  <AppCheckbox
    label="Certificar digitalmente el documento de respuesta"
    checked={certificaDigitalmente}
    onChange={(checked) => setCertificaDigitalmente(checked)}
    size="md"
  />
</div>
```


## REGLAS DE UI (OBLIGATORIAS)

### 1. Layout

- mantener el bloque de checks en columna
- spacing vertical consistente
- no romper el layout actual del modal
- no desplazar selects, `infoBox`, tags ni acciones

### 2. Consistencia visual

- usar `AppCheckbox size="md"` salvo una razon fuerte para variar
- respetar tipografia, espaciado y click target del shared
- evitar overrides agresivos sobre el checkbox de Ant Design
- si se ajusta el contenedor, el cambio debe ser del modulo, no del shared

### 3. Jerarquia visual

- los tres checkboxes siguen siendo el bloque superior del formulario
- deben verse como decisiones de confirmacion claras
- el texto debe quedar legible y alineado
- no se debe introducir ruido visual adicional

### 4. Responsive

- en mobile y tablet el bloque debe apilar correctamente
- los labels largos deben wrappear sin romper el click target
- el modal no debe generar overflow lateral por el texto de los checks


## ACCESIBILIDAD (OBLIGATORIA)

- mantener labels visibles y clicables
- conservar foco inicial del modal
- navegacion por teclado funcional dentro del bloque de checks
- respetar estados `checked` y `disabled` del shared
- no romper atributos accesibles del modal


## REGLAS DE IMPLEMENTACION

- no usar `any`
- no usar `AppInput type="checkbox"` en este modal despues del cambio
- usar `AppCheckbox` desde la capa shared
- no duplicar wrappers de checkbox en el modulo
- no usar estilos globales
- CSS Modules obligatorio
- mantener estado local desacoplado del backend
- preferir nombres de estado semanticos en vez de nombres heredados ambiguos


## ESTADO LOCAL SUGERIDO

El modal debe mantener estado local booleando y claro:

```ts
const [solicitaCentroEnvio, setSolicitaCentroEnvio] = useState(false);
const [confirmaCorreoPeticionario, setConfirmaCorreoPeticionario] = useState(true);
const [certificaDigitalmente, setCertificaDigitalmente] = useState(false);
```

Si no se renombra en esta FE, al menos debe mantenerse la misma semantica de
los estados actuales.


## CRITERIOS DE ACEPTACION

- el modal renderiza los tres checks con `AppCheckbox`
- ya no existe `AppInput type="checkbox"` en `GestionDocumentoModal.tsx`
- el estado local del modal sigue funcionando
- el layout del modal se mantiene estable
- la UI del bloque de checks se ve consistente con `AppInputSelect` y `AppInputTags`
- el comportamiento accesible no se degrada


## PRUEBAS OBLIGATORIAS

- renderiza los tres `AppCheckbox`
- cada checkbox refleja su estado inicial
- cambiar cada checkbox actualiza el estado local esperado
- el modal sigue abriendo y cerrando correctamente
- el contenido general del modal no se rompe por la migracion del bloque superior


## PRUEBAS MINIMAS SUGERIDAS

- validar presencia de los tres labels finales:
  - `Solicita al centro de envio de correspondencia el envio de la respuesta`
  - `Confirma respuesta al correo electronico del peticionario`
  - `Certificar digitalmente el documento de respuesta`
- validar que el checkbox usa el rol accesible correcto
- validar que el modal conserva `AppInputSelect`, `AppInputTags` y las acciones


## RESULTADO ESPERADO

El modal de gestion documental de `GestionRespuesta` debe quedar alineado con
la familia shared `AppCheckbox`, eliminando la dependencia de
`AppInput type="checkbox"` para este caso y consolidando una interfaz mas
consistente, reusable y mantenible.

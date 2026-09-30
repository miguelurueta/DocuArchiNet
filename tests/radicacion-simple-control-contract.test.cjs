const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

function sliceBetween(source, startMarker, endMarker) {
  const start = source.indexOf(startMarker);
  const end = source.indexOf(endMarker, start + startMarker.length);

  assert.ok(start >= 0, `No se encontró ${startMarker}`);
  assert.ok(end > start, `No se encontró ${endMarker}`);
  return source.slice(start, end);
}

test('el constructor de Radicación Simplificada cumple el contrato del validador general', () => {
  const page = read('js', 'RadicadorSimplificado', 'Web_form_radicacion_simpilificada.js');
  const validator = read('js', 'java_general', 'general_control_java.js');
  const constructor = sliceBetween(
    page,
    'const Create_interface_radicacion_simplificada',
    'const EventActualizaControlTom'
  );
  const validation = sliceBetween(
    validator,
    'function valida_solicita_datos_control_general',
    'const Tom_Set_item_Aray_G'
  );
  const requiredAttributes = Array.from(
    validation.matchAll(/attributes\["([^"]+)"\]\.value/g),
    match => match[1]
  );

  assert.ok(requiredAttributes.length > 0, 'el validador debe declarar atributos obligatorios');
  for (const attribute of new Set(requiredAttributes)) {
    assert.match(
      constructor,
      new RegExp(`setAttribute\\("${attribute}"\\s*,`, 'i'),
      `falta materializar el atributo requerido ${attribute}`
    );
  }
});

test('el constructor compartido de formularios cumple el contrato del validador general', () => {
  const source = read('js', 'java_general', 'general_control_java.js');
  const constructor = sliceBetween(
    source,
    'function Create_interface_formulario_control',
    'function search_valor_campo_form_control'
  );
  const validation = sliceBetween(
    source,
    'function valida_solicita_datos_control_general',
    'const Tom_Set_item_Aray_G'
  );
  const requiredAttributes = Array.from(
    validation.matchAll(/attributes\["([^"]+)"\]\.value/g),
    match => match[1]
  );

  assert.ok(requiredAttributes.length > 0, 'el validador debe declarar atributos obligatorios');
  for (const attribute of new Set(requiredAttributes)) {
    assert.match(
      constructor,
      new RegExp(`setAttribute\\("${attribute}"\\s*,`, 'i'),
      `falta materializar el atributo requerido ${attribute}`
    );
  }
});

test('la página invalida la caché del constructor corregido', () => {
  const page = read('RadicadorSimplificado', 'Web_form_radicacion_simpilificada.aspx');

  assert.match(
    page,
    /Web_form_radicacion_simpilificada\.js\?v=20260930-form-state-isolation1/
  );
  assert.match(
    page,
    /general_control_java\.js\?v=20260930-form-metadata1/
  );
});

test('la cascada trámite-flujo conserva la configuración del formulario que la creó', () => {
  const page = read('js', 'RadicadorSimplificado', 'Web_form_radicacion_simpilificada.js');
  const shared = read('js', 'java_general', 'general_control_java.js');
  const constructor = sliceBetween(
    page,
    'const Create_interface_radicacion_simplificada',
    'const EventActualizaControlTom'
  );
  const changeHandler = sliceBetween(
    shared,
    'function event_change_drowslis_form',
    'const Drow_confing_service'
  );

  assert.match(constructor, /const formControlConfiguration = ITEM_GENERAL_CONTROL_ARRAY_ASING\.slice\(\)/);
  assert.match(
    constructor,
    /event_change_drowslis_form\(event, formControlConfiguration\)/
  );
  assert.match(changeHandler, /Array\.isArray\(control_configuration\)/);
  assert.match(
    changeHandler,
    /Drow_confing_service\(atrrib_name_control_destino, form_control_configuration\)/
  );
  assert.match(
    changeHandler,
    /Drow_delete_rows\(atrrib_name_espace_control, atrrib_name_control, form_control_configuration\)/
  );
});

test('la cascada trámite-flujo no usa la configuración global reemplazada por el solicitante', () => {
  const shared = read('js', 'java_general', 'general_control_java.js');
  const cascadeFunctions = sliceBetween(
    shared,
    'function event_change_drowslis_form',
    '//  ZONA FORMAT DATE'
  );
  const flowConfiguration = [
    {
      name_campo: 'RE_flujo_trabajo',
      config_service_drowlis_destino: [{ service: 'flujo-por-tramite' }]
    },
    {
      name_campo: 'RE_flujo_trabajo',
      drow_name_padre_control: 'Descripcion_Documento'
    }
  ];
  let serviceCall;
  const context = {
    ITEM_GENERAL_CONTROL_ARRAY_ASING: [{ name_campo: 'campo_solicitante' }],
    document: { getElementById: () => ({}) },
    $: () => ({ empty: () => {} }),
    service_source_ilist_drow_control_general: (value, configuration, control) => {
      serviceCall = { value, configuration, control };
    }
  };

  vm.runInNewContext(cascadeFunctions, context);
  context.event_change_drowslis_form({
    currentTarget: {
      value: '42',
      attributes: {
        atrib_campo_drow_destino: { nodeValue: 'RE_flujo_trabajo' },
        atrib_name_espace_control: { nodeValue: 'content_radicacion_simplificada' },
        atrib_campo_n: { nodeValue: 'Descripcion_Documento' }
      }
    }
  }, flowConfiguration);

  assert.equal(serviceCall.value, '42');
  assert.equal(serviceCall.control, 'RE_flujo_trabajo_content_radicacion_simplificada');
  assert.equal(serviceCall.configuration[0].service, 'flujo-por-tramite');
});

test('la edición del tercero conserva la columna primaria y el id persistente', () => {
  const shared = read('js', 'java_general', 'general_control_java.js');
  const validator = sliceBetween(
    shared,
    'function valida_solicita_datos_control_general',
    'const Tom_Set_item_Aray_G'
  );
  const attributes = {
    atrib_aleas_c: { value: 'Nombre remitente' },
    atrib_campo_O: { value: '0' },
    atrib_campo_n: { value: 'Nombre_Remitente' },
    atrib_campo_v: { value: '0' },
    atrib_campo_nl: { value: '0' },
    atrib_campo_tip: { value: '1' },
    atrib_campo_id: { value: '73' },
    atrib_campo_t: { value: 'VARCHAR' },
    atrib_campo_tbl: { value: 'terceros' },
    atrib_control_tip_correo: { value: '0' },
    atrib_name_campo_id: { value: 'IdRemitente' },
    atrib_drow_name_control_id: { value: '' },
    atrib_value_campo_old: { value: 'Anterior' },
    atrib_campo_beetwen: { value: '0' },
    atrib_tom_alow: { value: 'null' }
  };
  const control = {
    id: 'Nombre_Remitente_actualizacion_validacion_externo',
    tagName: 'INPUT',
    value: 'Actualizado',
    maxLength: 100,
    attributes,
    focus: () => {}
  };
  const context = {
    ITEM_GENERAL_CONTROL_ARRAY: [],
    document: {
      getElementsByClassName: () => [control],
      getElementById: () => null
    },
    element_alert_clear_class: () => {},
    element_alert: () => {},
    validate_email_form_control: () => 'YES',
    validate_fecha_focus: () => 'YES'
  };

  vm.runInNewContext(validator, context);
  assert.equal(context.valida_solicita_datos_control_general('actualizacion_validacion_externo'), 'YES');
  assert.equal(context.ITEM_GENERAL_CONTROL_ARRAY[0].name_campo_id, 'IdRemitente');
  assert.equal(context.ITEM_GENERAL_CONTROL_ARRAY[0].dms_id_registro, '73');
  assert.notEqual(
    context.ITEM_GENERAL_CONTROL_ARRAY[0].name_campo_id,
    control.id
  );
});

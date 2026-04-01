import 'intl-tel-input/build/css/intlTelInput.css';
import intlTelInput from 'intl-tel-input/intlTelInputWithUtils';

const instances = new WeakMap();

function getInitialCountry() {
  const language = globalThis.navigator?.language ?? '';
  const parts = language.split('-');
  return parts.length > 1 ? parts.at(-1).toLowerCase() : '';
}

function buildPayload(input, iti) {
  const countryData = iti.getSelectedCountryData();
  const value = input.value ?? '';
  const hasValue = value.trim().length > 0;
  const isValid = hasValue && iti.isValidNumber();

  return {
    input: value,
    countryIso2: countryData?.iso2 ?? '',
    countryName: countryData?.name ?? '',
    dialCode: countryData?.dialCode ?? '',
    e164: isValid ? iti.getNumber() : '',
    isValid,
  };
}

async function notify(dotNetRef, input, iti) {
  const payload = buildPayload(input, iti);
  await dotNetRef.invokeMethodAsync('OnPhoneChangedAsync', payload);
}

export const gizmoCreateAccountPhone = {
  async init(input, dotNetRef) {
    const existing = instances.get(input);
    if (existing) {
      input.removeEventListener('input', existing.onChange);
      input.removeEventListener('countrychange', existing.onChange);
      existing.iti.destroy();
      instances.delete(input);
    }

    const iti = intlTelInput(input, {
      initialCountry: getInitialCountry(),
      nationalMode: true,
      separateDialCode: true,
      strictMode: true,
      autoPlaceholder: 'aggressive',
      dropdownContainer: document.body,
    });

    const onChange = async () => {
      await notify(dotNetRef, input, iti);
    };

    input.addEventListener('input', onChange);
    input.addEventListener('countrychange', onChange);

    instances.set(input, { iti, onChange });

    await notify(dotNetRef, input, iti);
  },

  destroy(input) {
    const instance = instances.get(input);
    if (!instance) {
      return;
    }

    input.removeEventListener('input', instance.onChange);
    input.removeEventListener('countrychange', instance.onChange);
    instance.iti.destroy();
    instances.delete(input);
  },
};

window.gizmoCreateAccountPhone = gizmoCreateAccountPhone;

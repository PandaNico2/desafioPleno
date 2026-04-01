window.lancamentoForm = function () {
    const tipoSelect = document.querySelector("[data-tipo-selector]");
    if (!tipoSelect) {
        return;
    }

    const taxaGroup = document.querySelector("[data-field-group='taxa']");
    const descontoGroup = document.querySelector("[data-field-group='desconto']");
    const taxaInput = taxaGroup ? taxaGroup.querySelector("input") : null;
    const descontoInput = descontoGroup ? descontoGroup.querySelector("input") : null;
    const tipoHelper = document.querySelector("[data-tipo-helper]");

    const creditoValue = "1";
    const debitoValue = "2";

    const updateVisibility = () => {
        const tipo = tipoSelect.value;

        if (tipoHelper) {
            tipoHelper.textContent = tipo === debitoValue
                ? "Debito selecionado: informe a taxa e deixe o desconto vazio."
                : "Credito selecionado: informe o desconto e deixe a taxa vazia.";
        }

        if (taxaGroup && taxaInput) {
            const showTaxa = tipo === debitoValue;
            taxaGroup.classList.toggle("field-hidden", !showTaxa);
            taxaInput.disabled = !showTaxa;
            if (showTaxa && !taxaInput.value) {
                taxaInput.value = "0";
            }
            if (!showTaxa) {
                taxaInput.value = "";
            }
        }

        if (descontoGroup && descontoInput) {
            const showDesconto = tipo === creditoValue;
            descontoGroup.classList.toggle("field-hidden", !showDesconto);
            descontoInput.disabled = !showDesconto;
            if (!showDesconto) {
                descontoInput.value = "";
            }
        }
    };

    tipoSelect.addEventListener("change", updateVisibility);
    updateVisibility();
};

window.inputMasks = function () {
    const competenciaInputs = document.querySelectorAll("[data-mask='competencia']");
    const decimalInputs = document.querySelectorAll("[data-mask='decimal']");

    competenciaInputs.forEach((input) => {
        const applyMask = () => {
            const digits = input.value.replace(/\D/g, "").slice(0, 6);
            if (digits.length <= 4) {
                input.value = digits;
                return;
            }

            input.value = `${digits.slice(0, 4)}-${digits.slice(4)}`;
        };

        input.addEventListener("input", applyMask);
        applyMask();
    });

    decimalInputs.forEach((input) => {
        const sanitizeDecimal = () => {
            const raw = input.value.replace(/[^0-9.,]/g, "");
            let result = "";
            let separatorUsed = false;
            let decimals = 0;

            for (const char of raw) {
                const isSeparator = char === "," || char === ".";

                if (isSeparator) {
                    if (separatorUsed) {
                        continue;
                    }

                    separatorUsed = true;
                    result += char;
                    continue;
                }

                if (separatorUsed) {
                    if (decimals >= 2) {
                        continue;
                    }

                    decimals++;
                }

                result += char;
            }

            input.value = result;
        };

        const enforceRange = () => {
            if (!input.value) {
                return;
            }

            const normalized = input.value.replace(",", ".");
            const parsed = Number(normalized);
            if (Number.isNaN(parsed)) {
                return;
            }

            const min = input.dataset.min ? Number(input.dataset.min) : null;
            const max = input.dataset.max ? Number(input.dataset.max) : null;

            let adjusted = parsed;
            if (min !== null && adjusted < min) {
                adjusted = min;
            }
            if (max !== null && adjusted > max) {
                adjusted = max;
            }

            if (adjusted !== parsed) {
                input.value = adjusted.toString().replace(".", ",");
            }
        };

        input.addEventListener("input", sanitizeDecimal);
        input.addEventListener("blur", enforceRange);
        sanitizeDecimal();
    });
};

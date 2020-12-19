﻿import { element, registerEditor } from "../../Decorators";
import { IStringValue } from "../../Interfaces/IStringValue";
import { text } from "../../Q/LocalText";
import { isEmptyOrNull } from "../../Q/Strings";
import { addValidationRule } from "../../Q/Validation";
import { Widget } from "../Widgets/Widget";

export interface RecaptchaOptions {
    siteKey?: string;
    language?: string;
}

@registerEditor('Serenity.Recaptcha', [IStringValue])
@element("<div/>")
export class Recaptcha extends Widget<RecaptchaOptions> implements IStringValue {
    constructor(div: JQuery, opt: RecaptchaOptions) {
        super(div, opt);

        this.element.addClass('g-recaptcha').attr('data-sitekey', this.options.siteKey);
        if (!!(window['grecaptcha'] == null && $('script#RecaptchaInclude').length === 0)) {
            var src = 'https://www.google.com/recaptcha/api.js';
            var lng = this.options.language;
            if (lng == null) {
                lng = $('html').attr('lang') ?? '';
            }
            src += '?hl=' + lng;
            $('<script/>').attr('id', 'RecaptchaInclude').attr('src', src).appendTo(document.body);
        }

        var valInput = $('<input />').insertBefore(this.element)
            .attr('id', this.uniqueName + '_validate').val('x');

        var gro = {};
        gro['visibility'] = 'hidden';
        gro['width'] = '0px';
        gro['height'] = '0px';
        gro['padding'] = '0px';

        var input = valInput.css(gro);
        var self = this;

        addValidationRule(input, this.uniqueName, e => {
            if (isEmptyOrNull(this.get_value())) {
                return text('Validation.Required');
            }
            return null;
        });
    }

    get_value(): string {
        return this.element.find('.g-recaptcha-response').val();
    }

    set_value(value: string): void {
        // ignore
    }
}
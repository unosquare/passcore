import * as zxcvbn from 'zxcvbn';

export class PasswordStrength {
    static measureStrength(p: string) {
        return Math.min(zxcvbn(p).guesses_log10 * 10, 100);
    }
}

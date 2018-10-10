export class PasswordStrength{
    
    static measureStrength(p: string) {
        let force = 0;
        const regex = /[$-/:-?{-~!"^_`\[\]]/g;

        const lowerLetters = /[a-z]+/.test(p);
        const upperLetters = /[A-Z]+/.test(p);
        const numbers = /[0-9]+/.test(p);
        const symbols = regex.test(p);

        const flags = [lowerLetters, upperLetters, numbers, symbols];

        let passedMatches = 0;
        for (let flag of flags) {
            passedMatches += flag ? 1 : 0;
        }

        force += 2 * p.length + ((p.length >= 10) ? 1 : 0);
        force += passedMatches * 10;

        // penalty (short password)
        force = (p.length <= 6) ? Math.min(force, 10) : force;

        // penalty (poor variety of characters)
        force = (passedMatches === 1) ? Math.min(force, 10) : force;
        force = (passedMatches === 2) ? Math.min(force, 20) : force;
        force = (passedMatches === 3) ? Math.min(force, 40) : force;

        return force;
    }

}
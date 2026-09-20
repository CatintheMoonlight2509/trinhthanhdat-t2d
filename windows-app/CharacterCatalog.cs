using System.Collections.Generic;

namespace CMkeyCombat
{
    sealed class CharacterCatalogEntry
    {
        public readonly char Source;
        public readonly string DefaultReplacement;
        public readonly string Variants;
        public CharacterCatalogEntry(char source, string defaultReplacement, string variants)
        { Source = source; DefaultReplacement = defaultReplacement; Variants = variants; }
    }

    static class CharacterCatalog
    {
        public static readonly CharacterCatalogEntry[] Entries = new CharacterCatalogEntry[]
        {
            new CharacterCatalogEntry('A', CombatMap.TransformDefault('A') ?? "A", "Α — GREEK CAPITAL LETTER ALPHA; ᗅ — CANADIAN SYLLABICS CARRIER GHO; Λ — GREEK CAPITAL LETTER LAMDA; ᗋ — CANADIAN SYLLABICS CARRIER RO; Ꭿ — CHEROKEE LETTER HI; ᗩ — CANADIAN SYLLABICS CARRIER PO; ₳ — AUSTRAL SIGN;"),
            new CharacterCatalogEntry('a', CombatMap.TransformDefault('a') ?? "a", "а — CYRILLIC SMALL LETTER A; ɑ — LATIN SMALL LETTER ALPHA; 𝔞 — MATHEMATICAL FRAKTUR SMALL A; ᥑ — TAI LE LETTER XA; ⩜ — LOGICAL AND WITH HORIZONTAL DASH;"),
            new CharacterCatalogEntry('B', CombatMap.TransformDefault('B') ?? "B", "Β — GREEK CAPITAL LETTER BETA; В — CYRILLIC CAPITAL LETTER VE; ₿ — BITCOIN SIGN; β — GREEK SMALL LETTER BETA; Ᏸ — CHEROKEE LETTER YE; ᗷ — CANADIAN SYLLABICS CARRIER KHE;"),
            new CharacterCatalogEntry('b', CombatMap.TransformDefault('b') ?? "b", "ᏸ — CHEROKEE SMALL LETTER YE; ᑲ — CANADIAN SYLLABICS KA; Ь — CYRILLIC CAPITAL LETTER SOFT SIGN; ь — CYRILLIC SMALL LETTER SOFT SIGN; в — CYRILLIC SMALL LETTER VE; ⲃ — COPTIC SMALL LETTER VIDA; ᏼ — CHEROKEE SMALL LETTER YV;"),
            new CharacterCatalogEntry('C', CombatMap.TransformDefault('C') ?? "C", "Ϲ — GREEK CAPITAL LUNATE SIGMA SYMBOL; С — CYRILLIC CAPITAL LETTER ES; Ⅽ — ROMAN NUMERAL ONE HUNDRED; Ҫ — CYRILLIC CAPITAL LETTER ES WITH DESCENDER;"),
            new CharacterCatalogEntry('c', CombatMap.TransformDefault('c') ?? "c", "ϲ — GREEK LUNATE SIGMA SYMBOL; с — CYRILLIC SMALL LETTER ES; ᥴ — TAI LE LETTER TONE-6; ⅽ — SMALL ROMAN NUMERAL ONE HUNDRED;"),
            new CharacterCatalogEntry('D', CombatMap.TransformDefault('D') ?? "D", "ᗪ — CANADIAN SYLLABICS CARRIER PE; ᗞ — CANADIAN SYLLABICS CARRIER THE;"),
            new CharacterCatalogEntry('d', CombatMap.TransformDefault('d') ?? "d", "Ԁ — CYRILLIC CAPITAL LETTER KOMI DE; ԁ — CYRILLIC SMALL LETTER KOMI DE; ᑯ — CANADIAN SYLLABICS KO; 𐓰 — OSAGE SMALL LETTER TA; ძ — GEORGIAN LETTER JIL;"),
            new CharacterCatalogEntry('Đ', CombatMap.TransformDefault('Đ') ?? "Đ", "Ɖ — LATIN CAPITAL LETTER AFRICAN D;"),
            new CharacterCatalogEntry('đ', CombatMap.TransformDefault('đ') ?? "đ", "ᵭ — LATIN SMALL LETTER D WITH MIDDLE TILDE; ₫ — DONG SIGN; ƌ — LATIN SMALL LETTER D WITH TOPBAR; 𐓱 — OSAGE SMALL LETTER EHTA; ᴆ — LATIN LETTER SMALL CAPITAL ETH; Ꮷ — CHEROKEE LETTER TSU; ð — LATIN SMALL LETTER ETH;"),
            new CharacterCatalogEntry('E', CombatMap.TransformDefault('E') ?? "E", "Ԑ — CYRILLIC CAPITAL LETTER REVERSED ZE; Ε — GREEK CAPITAL LETTER EPSILON;"),
            new CharacterCatalogEntry('e', CombatMap.TransformDefault('e') ?? "e", "ᥱ — TAI LE LETTER TONE-3; ᧉ — NEW TAI LUE TONE MARK-2; ԑ — CYRILLIC SMALL LETTER REVERSED ZE; ε — GREEK SMALL LETTER EPSILON; ϵ — GREEK LUNATE EPSILON SYMBOL; є — CYRILLIC SMALL LETTER UKRAINIAN IE;"),
            new CharacterCatalogEntry('F', CombatMap.TransformDefault('F') ?? "F", "Ϝ — GREEK LETTER DIGAMMA; Ғ — CYRILLIC CAPITAL LETTER GHE WITH STROKE; ᖴ — CANADIAN SYLLABICS BLACKFOOT WE;"),
            new CharacterCatalogEntry('f', CombatMap.TransformDefault('f') ?? "f", "𑫚 — PAU CIN HAU LETTER UA; ϝ — GREEK SMALL LETTER DIGAMMA; ғ — CYRILLIC SMALL LETTER GHE WITH STROKE;"),
            new CharacterCatalogEntry('G', CombatMap.TransformDefault('G') ?? "G", "Ԍ — CYRILLIC CAPITAL LETTER KOMI SJE; Ⴚ — GEORGIAN CAPITAL LETTER CAN; Ᏽ — CHEROKEE LETTER MV; Ꮐ — CHEROKEE LETTER NAH; ₲ — GUARANI SIGN;"),
            new CharacterCatalogEntry('g', CombatMap.TransformDefault('g') ?? "g", "ԍ — CYRILLIC SMALL LETTER KOMI SJE; ց — ARMENIAN SMALL LETTER CO; 𝔤 — MATHEMATICAL FRAKTUR SMALL G; ᏽ — CHEROKEE SMALL LETTER MV;"),
            new CharacterCatalogEntry('H', CombatMap.TransformDefault('H') ?? "H", "Η — GREEK CAPITAL LETTER ETA; Н — CYRILLIC CAPITAL LETTER EN; Ң — CYRILLIC CAPITAL LETTER EN WITH DESCENDER; Ҥ — CYRILLIC CAPITAL LIGATURE EN GHE; ዘ — ETHIOPIC SYLLABLE ZA;"),
            new CharacterCatalogEntry('h', CombatMap.TransformDefault('h') ?? "h", "н — CYRILLIC SMALL LETTER EN; ң — CYRILLIC SMALL LETTER EN WITH DESCENDER; ҥ — CYRILLIC SMALL LIGATURE EN GHE; ⲏ — COPTIC SMALL LETTER HATE; Ꮒ — CHEROKEE LETTER NI; ዞ — ETHIOPIC SYLLABLE ZO;"),
            new CharacterCatalogEntry('I', CombatMap.TransformDefault('I') ?? "I", "Ι — GREEK CAPITAL LETTER IOTA; Ⲓ — COPTIC CAPITAL LETTER IAUDA;"),
            new CharacterCatalogEntry('i', CombatMap.TransformDefault('i') ?? "i", "ι — GREEK SMALL LETTER IOTA; ɩ — LATIN SMALL LETTER IOTA; Ꭵ — CHEROKEE LETTER V; ї — CYRILLIC SMALL LETTER YI; ⲓ — COPTIC SMALL LETTER IAUDA; 𝜄 — MATHEMATICAL ITALIC SMALL IOTA; เ — THAI CHARACTER SARA E;"),
            new CharacterCatalogEntry('J', CombatMap.TransformDefault('J') ?? "J", "Ј — CYRILLIC CAPITAL LETTER JE; յ — ARMENIAN SMALL LETTER YI; Ϳ — GREEK CAPITAL LETTER YOT; Ꭻ — CHEROKEE LETTER GU;"),
            new CharacterCatalogEntry('j', CombatMap.TransformDefault('j') ?? "j", "ϳ — GREEK LETTER YOT; ј — CYRILLIC SMALL LETTER JE;"),
            new CharacterCatalogEntry('K', CombatMap.TransformDefault('K') ?? "K", "Κ — GREEK CAPITAL LETTER KAPPA; Қ — CYRILLIC CAPITAL LETTER KA WITH DESCENDER; ₭ — KIP SIGN; Ⲕ — COPTIC CAPITAL LETTER KAPA; Ꮶ — CHEROKEE LETTER TSO;"),
            new CharacterCatalogEntry('k', CombatMap.TransformDefault('k') ?? "k", "ⲕ — COPTIC SMALL LETTER KAPA; κ — GREEK SMALL LETTER KAPPA; к — CYRILLIC SMALL LETTER KA; қ — CYRILLIC SMALL LETTER KA WITH DESCENDER;"),
            new CharacterCatalogEntry('L', CombatMap.TransformDefault('L') ?? "L", "ᒪ — CANADIAN SYLLABICS MA; Ꮮ — CHEROKEE LETTER TLE; ᥨ — TAI LE LETTER OO; Լ — ARMENIAN CAPITAL LETTER LIWN; Ⅼ — ROMAN NUMERAL FIFTY;"),
            new CharacterCatalogEntry('l', CombatMap.TransformDefault('l') ?? "l", "ւ — ARMENIAN SMALL LETTER YIWN; 𝔩 — MATHEMATICAL FRAKTUR SMALL L; ℓ — SCRIPT SMALL L; ⅼ — SMALL ROMAN NUMERAL FIFTY;"),
            new CharacterCatalogEntry('M', CombatMap.TransformDefault('M') ?? "M", "Μ — GREEK CAPITAL LETTER MU; Ϻ — GREEK CAPITAL LETTER SAN; Ⲙ — COPTIC CAPITAL LETTER MI; ᙏ — CANADIAN SYLLABICS CARRIER SO;"),
            new CharacterCatalogEntry('m', CombatMap.TransformDefault('m') ?? "m", "ϻ — GREEK SMALL LETTER SAN; ʍ — LATIN SMALL LETTER TURNED W; ⲙ — COPTIC SMALL LETTER MI; ꪔ — TAI VIET LETTER LOW TO;"),
            new CharacterCatalogEntry('N', CombatMap.TransformDefault('N') ?? "N", "Ν — GREEK CAPITAL LETTER NU; Ͷ — GREEK CAPITAL LETTER PAMPHYLIAN DIGAMMA; 𑪾 — CANADIAN SYLLABICS SPO; Ⲛ — COPTIC CAPITAL LETTER NI;"),
            new CharacterCatalogEntry('n', CombatMap.TransformDefault('n') ?? "n", "ⲛ — COPTIC SMALL LETTER NI; ͷ — GREEK SMALL LETTER PAMPHYLIAN DIGAMMA; ո — ARMENIAN SMALL LETTER VO; η — GREEK SMALL LETTER ETA; ռ — ARMENIAN SMALL LETTER RA; ⴖ — GEORGIAN SMALL LETTER GHAN; ᥒ — TAI LE LETTER NGA; ท — THAI CHARACTER THO THAHAN;"),
            new CharacterCatalogEntry('O', CombatMap.TransformDefault('O') ?? "O", "ϴ — GREEK CAPITAL THETA SYMBOL; Ο — GREEK CAPITAL LETTER OMICRON; θ — GREEK SMALL LETTER THETA; Ѻ — CYRILLIC CAPITAL LETTER ROUND OMEGA;"),
            new CharacterCatalogEntry('o', CombatMap.TransformDefault('o') ?? "o", "ο — GREEK SMALL LETTER OMICRON; σ — GREEK SMALL LETTER SIGMA; ᦞ — NEW TAI LUE LETTER LOW VA; ѻ — CYRILLIC SMALL LETTER ROUND OMEGA; ໐ — LAO DIGIT ZERO;"),
            new CharacterCatalogEntry('P', CombatMap.TransformDefault('P') ?? "P", "Ρ — GREEK CAPITAL LETTER RHO; Р — CYRILLIC CAPITAL LETTER ER; ᑭ — CANADIAN SYLLABICS KI; Ҏ — CYRILLIC CAPITAL LETTER ER WITH TICK;"),
            new CharacterCatalogEntry('p', CombatMap.TransformDefault('p') ?? "p", "𝜌 — MATHEMATICAL ITALIC SMALL RHO; ҏ — CYRILLIC SMALL LETTER ER WITH TICK; ρ — GREEK SMALL LETTER RHO; ᑭ — CANADIAN SYLLABICS KI;"),
            new CharacterCatalogEntry('Q', CombatMap.TransformDefault('Q') ?? "Q", "Ԛ — CYRILLIC CAPITAL LETTER QA; ⵕ — TIFINAGH LETTER YARR;"),
            new CharacterCatalogEntry('q', CombatMap.TransformDefault('q') ?? "q", "ᤚ — LIMBU LETTER SSA; ᨾ — TAI THAM LETTER MA; ᑫ — CANADIAN SYLLABICS KE; ԛ — CYRILLIC SMALL LETTER QA; գ — ARMENIAN SMALL LETTER GIM;"),
            new CharacterCatalogEntry('R', CombatMap.TransformDefault('R') ?? "R", "Я — CYRILLIC CAPITAL LETTER YA; R — LATIN CAPITAL LETTER R; Ʀ — LATIN LETTER YR; Ꮢ — CHEROKEE LETTER SV; ᖇ — CANADIAN SYLLABICS TLHI; Ꭱ — CHEROKEE LETTER E;"),
            new CharacterCatalogEntry('r', CombatMap.TransformDefault('r') ?? "r", "я — CYRILLIC SMALL LETTER YA; ʀ — LATIN LETTER SMALL CAPITAL R; г — CYRILLIC SMALL LETTER GHE;"),
            new CharacterCatalogEntry('S', CombatMap.TransformDefault('S') ?? "S", "Ѕ — CYRILLIC CAPITAL LETTER DZE; Տ — ARMENIAN CAPITAL LETTER TIWN; ჽ — GEORGIAN LETTER AEN; Ꮪ — CHEROKEE LETTER DU; $ — DOLLAR SIGN;"),
            new CharacterCatalogEntry('s', CombatMap.TransformDefault('s') ?? "s", "ѕ — CYRILLIC SMALL LETTER DZE; ᥉ — LIMBU DIGIT THREE; ട — MALAYALAM LETTER TTA; ຣ — LAO LETTER LO LING;"),
            new CharacterCatalogEntry('T', CombatMap.TransformDefault('T') ?? "T", "Ͳ — GREEK CAPITAL LETTER ARCHAIC SAMPI; Т — CYRILLIC CAPITAL LETTER TE; Τ — GREEK CAPITAL LETTER TAU; Ꭲ — CHEROKEE LETTER I; ₸ — TENGE SIGN; Ҭ — CYRILLIC CAPITAL LETTER TE WITH DESCENDER;"),
            new CharacterCatalogEntry('t', CombatMap.TransformDefault('t') ?? "t", "ϯ — COPTIC SMALL LETTER DEI; τ — GREEK SMALL LETTER TAU; ꚍ — CYRILLIC SMALL LETTER TWE; ҭ — CYRILLIC SMALL LETTER TE WITH DESCENDER;"),
            new CharacterCatalogEntry('U', CombatMap.TransformDefault('U') ?? "U", "Ա — ARMENIAN CAPITAL LETTER AYB; Ս — ARMENIAN CAPITAL LETTER SEH; ᥩ — TAI LE LETTER O; 𝈈 — GREEK VOCAL NOTATION SYMBOL-9;"),
            new CharacterCatalogEntry('u', CombatMap.TransformDefault('u') ?? "u", "υ — GREEK SMALL LETTER UPSILON; ᥙ — TAI LE LETTER PA; ս — ARMENIAN SMALL LETTER SEH; ບ — LAO LETTER BO;"),
            new CharacterCatalogEntry('V', CombatMap.TransformDefault('V') ?? "V", "Ѵ — CYRILLIC CAPITAL LETTER IZHITSA; 𝈍 — GREEK VOCAL NOTATION SYMBOL-14; Ꮴ — CHEROKEE LETTER TSE;"),
            new CharacterCatalogEntry('v', CombatMap.TransformDefault('v') ?? "v", "ν — GREEK SMALL LETTER NU; 𝜈 — MATHEMATICAL ITALIC SMALL NU; ѵ — CYRILLIC SMALL LETTER IZHITSA; ᥎ — LIMBU DIGIT EIGHT; ꪚ — TAI VIET LETTER LOW BO;"),
            new CharacterCatalogEntry('W', CombatMap.TransformDefault('W') ?? "W", "Ԝ — CYRILLIC CAPITAL LETTER WE; ᗯ — CANADIAN SYLLABICS CARRIER GU; Ꮃ — CHEROKEE LETTER LA; Ꮤ — CHEROKEE LETTER TA; ᙎ — CANADIAN SYLLABICS CARRIER SU;"),
            new CharacterCatalogEntry('w', CombatMap.TransformDefault('w') ?? "w", "ꪝ — TAI VIET LETTER HIGH PO; ԝ — CYRILLIC SMALL LETTER WE; ѡ — CYRILLIC SMALL LETTER OMEGA; ω — GREEK SMALL LETTER OMEGA; ധ — MALAYALAM LETTER DHA; 𝈢 — GREEK INSTRUMENTAL NOTATION SYMBOL-8;"),
            new CharacterCatalogEntry('X', CombatMap.TransformDefault('X') ?? "X", "Χ — GREEK CAPITAL LETTER CHI; Х — CYRILLIC CAPITAL LETTER HA; Ӽ — CYRILLIC CAPITAL LETTER HA WITH HOOK; Ⲭ — COPTIC CAPITAL LETTER KHI; ⵋ — TIFINAGH LETTER AHAGGAR YAZH; χ — GREEK SMALL LETTER CHI;"),
            new CharacterCatalogEntry('x', CombatMap.TransformDefault('x') ?? "x", "х — CYRILLIC SMALL LETTER HA; ӽ — CYRILLIC SMALL LETTER HA WITH HOOK; ᥊ — LIMBU DIGIT FOUR; ⲭ — COPTIC SMALL LETTER KHI; ꪎ — TAI VIET LETTER LOW SO; ӿ — CYRILLIC SMALL LETTER HA WITH STROKE;"),
            new CharacterCatalogEntry('Y', CombatMap.TransformDefault('Y') ?? "Y", "Υ — GREEK CAPITAL LETTER UPSILON; У — CYRILLIC CAPITAL LETTER U; ϒ — GREEK UPSILON WITH HOOK SYMBOL; Ⲩ — COPTIC CAPITAL LETTER UA; ⲩ — COPTIC SMALL LETTER UA;"),
            new CharacterCatalogEntry('y', CombatMap.TransformDefault('y') ?? "y", "γ — GREEK SMALL LETTER GAMMA; у — CYRILLIC SMALL LETTER U; ყ — GEORGIAN LETTER QAR; ⴘ — GEORGIAN SMALL LETTER SHIN;"),
            new CharacterCatalogEntry('Z', CombatMap.TransformDefault('Z') ?? "Z", "Ζ — GREEK CAPITAL LETTER ZETA; Ⴭ — GEORGIAN CAPITAL LETTER AEN; 𝛧 — MATHEMATICAL ITALIC CAPITAL ZETA; 𑪽 — CANADIAN SYLLABICS SPI;"),
            new CharacterCatalogEntry('z', CombatMap.TransformDefault('z') ?? "z", "ⲍ — COPTIC SMALL LETTER ZATA; ⴭ — GEORGIAN SMALL LETTER AEN; ᤏ — LIMBU LETTER NA; ƶ — LATIN SMALL LETTER Z WITH STROKE;"),
        };
    }
}

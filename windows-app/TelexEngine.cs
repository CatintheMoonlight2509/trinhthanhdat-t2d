// TelexEngine.cs — Port thuần logic từ TelexEngine.kt (bản Android CMkey).
// Chỉ port nhánh SỐNG: Translate -> ProcessSmartTelex + helpers.
// processCombatTelex bên Kotlin là code chết (translate() luôn gọi processSmartTelex),
// nên không port. Không phụ thuộc thư viện ngoài — build được bằng csc.exe.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMkeyCombat
{
    static class TelexEngine
    {
        // base char -> [khong dau, sac, huyen, hoi, nga, nang]
        static readonly Dictionary<char, char[]> vowelMap = new Dictionary<char, char[]>
        {
            {'a', new[]{'a','á','à','ả','ã','ạ'}},
            {'ă', new[]{'ă','ắ','ằ','ẳ','ẵ','ặ'}},
            {'â', new[]{'â','ấ','ầ','ẩ','ẫ','ậ'}},
            {'e', new[]{'e','é','è','ẻ','ẽ','ẹ'}},
            {'ê', new[]{'ê','ế','ề','ể','ễ','ệ'}},
            {'i', new[]{'i','í','ì','ỉ','ĩ','ị'}},
            {'o', new[]{'o','ó','ò','ỏ','õ','ọ'}},
            {'ô', new[]{'ô','ố','ồ','ổ','ỗ','ộ'}},
            {'ơ', new[]{'ơ','ớ','ờ','ở','ỡ','ợ'}},
            {'u', new[]{'u','ú','ù','ủ','ũ','ụ'}},
            {'ư', new[]{'ư','ứ','ừ','ử','ữ','ự'}},
            {'y', new[]{'y','ý','ỳ','ỷ','ỹ','ỵ'}},

            {'A', new[]{'A','Á','À','Ả','Ã','Ạ'}},
            {'Ă', new[]{'Ă','Ắ','Ằ','Ẳ','Ẵ','Ặ'}},
            {'Â', new[]{'Â','Ấ','Ầ','Ẩ','Ẫ','Ậ'}},
            {'E', new[]{'E','É','È','Ẻ','Ẽ','Ẹ'}},
            {'Ê', new[]{'Ê','Ế','Ề','Ể','Ễ','Ệ'}},
            {'I', new[]{'I','Í','Ì','Ỉ','Ĩ','Ị'}},
            {'O', new[]{'O','Ó','Ò','Ỏ','Õ','Ọ'}},
            {'Ô', new[]{'Ô','Ố','Ồ','Ổ','Ỗ','Ộ'}},
            {'Ơ', new[]{'Ơ','Ớ','Ờ','Ở','Ỡ','Ợ'}},
            {'U', new[]{'U','Ú','Ù','Ủ','Ũ','Ụ'}},
            {'Ư', new[]{'Ư','Ứ','Ừ','Ử','Ữ','Ự'}},
            {'Y', new[]{'Y','Ý','Ỳ','Ỷ','Ỹ','Ỵ'}}
        };

        // accented char -> (base char, tone index)
        static readonly Dictionary<char, KeyValuePair<char, int>> inverseVowelMap = BuildInverse();

        static Dictionary<char, KeyValuePair<char, int>> BuildInverse()
        {
            var map = new Dictionary<char, KeyValuePair<char, int>>();
            foreach (var kv in vowelMap)
            {
                var arr = kv.Value;
                for (int t = 0; t < arr.Length; t++)
                    map[arr[t]] = new KeyValuePair<char, int>(kv.Key, t);
            }
            return map;
        }

        static readonly HashSet<char> vowelSetHasVowel = new HashSet<char>(
            "aeiouyăâêôơưAEIOUYĂÂÊÔƠƯ".ToCharArray());
        static readonly HashSet<char> vowelSetLower = new HashSet<char>(
            "aeiouyăâêôơư".ToCharArray());
        static readonly HashSet<char> toneChars = new HashSet<char>(new[] { 's', 'f', 'r', 'x', 'j' });
        static readonly HashSet<char> hookBases = new HashSet<char>(new[] { 'ư', 'ơ', 'ă', 'Ư', 'Ơ', 'Ă' });

        static bool HasVowel(string s)
        {
            foreach (char c in s) if (vowelSetHasVowel.Contains(c)) return true;
            return false;
        }

        static bool IsVowel(char c) { return vowelSetLower.Contains(char.ToLowerInvariant(c)); }

        // ── Entry point: xử lý cả câu (tách theo dấu cách như Kotlin process()) ──
        public static string Process(string rawInput)
        {
            return string.Join(" ", rawInput.Split(' ').Select(Translate));
        }

        public static string Translate(string raw)
        {
            int len = raw.Length;
            if (len >= 3)
            {
                string last3 = raw.Substring(len - 3).ToLowerInvariant();
                if (last3 == "ooo" || last3 == "eee" || last3 == "aaa" || last3 == "ddd")
                    return Translate(raw.Substring(0, len - 3)) + raw[len - 2] + raw[len - 1];
                if (last3 == "aww" || last3 == "oww" || last3 == "uww")
                    return Translate(raw.Substring(0, len - 3)) + raw[len - 3] + raw[len - 2];
            }
            if (len >= 2)
            {
                string last2 = raw.Substring(len - 2).ToLowerInvariant();
                if (last2 == "ss" || last2 == "ff" || last2 == "rr" || last2 == "xx" || last2 == "jj")
                    return Translate(raw.Substring(0, len - 2)) + raw[len - 1];
            }
            return ProcessSmartTelex(raw);
        }

        static string ProcessSmartTelex(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";

            bool isFirstCharUpper = char.IsUpper(raw[0]);
            bool isAllUpper = raw.Length > 1 && raw.All(char.IsUpper);
            string word = raw.ToLowerInvariant();

            // 1. 4-char rule
            word = word.Replace("uoow", "ươu");
            // 2. 3-char rules
            word = word.Replace("uwu", "ưu");
            word = word.Replace("uuw", "ưu");
            word = word.Replace("owo", "ơo");
            word = word.Replace("oow", "ơo");
            word = word.Replace("uow", "ươ");
            word = word.Replace("ouw", "ươ");

            // 1. Decompose pre-existing accents
            int activeTone = 0;
            bool hasHook = false;
            var decompressed = new StringBuilder();
            foreach (char c in word)
            {
                KeyValuePair<char, int> mapping;
                if (inverseVowelMap.TryGetValue(c, out mapping))
                {
                    char baseChar = mapping.Key;
                    int toneIdx = mapping.Value;
                    decompressed.Append(baseChar);
                    if (toneIdx > 0) activeTone = toneIdx;
                    if (hookBases.Contains(baseChar)) hasHook = true;
                }
                else decompressed.Append(c);
            }
            word = decompressed.ToString();

            // 2. Parse left-to-right for freestyle tones/hooks
            var baseSb = new StringBuilder();
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                bool prefixHasVowel = HasVowel(word.Substring(0, i));
                if (prefixHasVowel && toneChars.Contains(c) && i > 0 && !(c == 'r' && i == 1 && word[0] == 't'))
                {
                    int cTone = c == 's' ? 1 : c == 'f' ? 2 : c == 'r' ? 3 : c == 'x' ? 4 : c == 'j' ? 5 : 0;
                    if (activeTone == cTone) { activeTone = 0; baseSb.Append(c); }
                    else { activeTone = cTone; }
                }
                else if (prefixHasVowel && c == 'w' && i > 0)
                {
                    if (word[i - 1] == 'w') { hasHook = false; baseSb.Append(c); }
                    else { hasHook = true; }
                }
                else baseSb.Append(c);
            }

            string baseWord = baseSb.ToString();

            // 3. inline double-tap modifiers
            baseWord = baseWord.Replace("aa", "â");
            baseWord = baseWord.Replace("ee", "ê");
            baseWord = baseWord.Replace("oo", "ô");
            baseWord = baseWord.Replace("dd", "đ");
            // hoàn lương
            baseWord = baseWord.Replace("âa", "aa");
            baseWord = baseWord.Replace("êe", "ee");
            baseWord = baseWord.Replace("ôo", "oo");
            baseWord = baseWord.Replace("đd", "dd");

            // 4. hook
            if (hasHook) baseWord = ApplyHookToVowelCluster(baseWord);
            // 5. tone
            if (activeTone > 0) baseWord = ApplyTone(baseWord, activeTone);

            // Restore capitalization
            if (isAllUpper) baseWord = baseWord.ToUpperInvariant();
            else if (isFirstCharUpper && baseWord.Length > 0)
                baseWord = char.ToUpperInvariant(baseWord[0]) + baseWord.Substring(1);

            return baseWord;
        }

        static string ToBaseVowels(string seq)
        {
            var sb = new StringBuilder(seq.Length);
            foreach (char c in seq)
            {
                switch (c)
                {
                    case 'ă': case 'â': sb.Append('a'); break;
                    case 'ê': sb.Append('e'); break;
                    case 'ô': case 'ơ': sb.Append('o'); break;
                    case 'ư': sb.Append('u'); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        static string ApplyHookToSeq(string vowelSeq)
        {
            string baseVowelSeq = ToBaseVowels(vowelSeq);

            foreach (var pair in new[] { "uo", "oa", "ua", "oe", "oo" })
            {
                int idx = baseVowelSeq.IndexOf(pair, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string rep = pair == "uo" ? "ươ" : pair == "oa" ? "oă"
                        : pair == "ua" ? "ưa" : pair == "oe" ? "oe" : "ơo";
                    var sb2 = new StringBuilder(vowelSeq);
                    sb2.Remove(idx, 2).Insert(idx, rep);
                    return sb2.ToString();
                }
            }

            bool hookedU = false;
            var sb = new StringBuilder(vowelSeq);
            for (int i = 0; i < vowelSeq.Length; i++)
            {
                char baseChar;
                switch (vowelSeq[i])
                {
                    case 'ă': case 'â': baseChar = 'a'; break;
                    case 'ê': baseChar = 'e'; break;
                    case 'ô': case 'ơ': baseChar = 'o'; break;
                    case 'ư': baseChar = 'u'; break;
                    default: baseChar = vowelSeq[i]; break;
                }
                if (baseChar == 'u') { if (!hookedU) { sb[i] = 'ư'; hookedU = true; } else sb[i] = 'u'; }
                else if (baseChar == 'o') sb[i] = 'ơ';
                else if (baseChar == 'a') sb[i] = 'ă';
            }
            return sb.ToString();
        }

        static string ApplyHookToVowelCluster(string word)
        {
            string lowerWord = word.ToLowerInvariant();
            int startIdx = 0;
            if (lowerWord.StartsWith("qu") && word.Length > 2) startIdx = 2;
            else if (lowerWord.StartsWith("gi") && word.Length > 2)
            {
                bool hasOtherVowel = false;
                for (int k = 2; k < word.Length; k++) if (IsVowel(word[k])) { hasOtherVowel = true; break; }
                if (hasOtherVowel) startIdx = 2;
            }

            int firstVowelIdx = -1, lastVowelIdx = -1;
            for (int i = startIdx; i < word.Length; i++)
            {
                if (IsVowel(word[i])) { if (firstVowelIdx == -1) firstVowelIdx = i; lastVowelIdx = i; }
                else if (firstVowelIdx != -1) break;
            }
            if (firstVowelIdx == -1) return word;

            string vowelSeq = word.Substring(firstVowelIdx, lastVowelIdx - firstVowelIdx + 1).ToLowerInvariant();
            string modified = ApplyHookToSeq(vowelSeq);
            var sb = new StringBuilder(word);
            sb.Remove(firstVowelIdx, lastVowelIdx - firstVowelIdx + 1).Insert(firstVowelIdx, modified);
            return sb.ToString();
        }

        static string ApplyTone(string word, int tone)
        {
            if (tone == 0) return word;

            var vowels = new List<KeyValuePair<int, char>>();
            string lowerWord = word.ToLowerInvariant();
            int startIdx = 0;
            if (lowerWord.StartsWith("qu") && word.Length > 2) startIdx = 2;
            else if (lowerWord.StartsWith("gi") && word.Length > 2)
            {
                bool hasOtherVowel = false;
                for (int k = 2; k < word.Length; k++) if (IsVowel(word[k])) { hasOtherVowel = true; break; }
                if (hasOtherVowel) startIdx = 2;
            }

            for (int k = startIdx; k < word.Length; k++)
                if (IsVowel(word[k])) vowels.Add(new KeyValuePair<int, char>(k, word[k]));

            if (vowels.Count == 0) return word;

            KeyValuePair<int, char> targetVowelPair;
            if (vowels.Count == 1) targetVowelPair = vowels[0];
            else if (vowels.Count == 2)
            {
                int lastVowelIdx = vowels[1].Key;
                bool endsWithConsonant = lastVowelIdx < word.Length - 1;
                if (endsWithConsonant) targetVowelPair = vowels[1];
                else
                {
                    string pair = "" + char.ToLowerInvariant(vowels[0].Value) + char.ToLowerInvariant(vowels[1].Value);
                    bool toneOnSecond = pair == "oa" || pair == "oe" || pair == "uy" || pair == "uơ" || pair == "uê";
                    targetVowelPair = toneOnSecond ? vowels[1] : vowels[0];
                }
            }
            else
            {
                var tri = FindTriphthongTargetIndex(vowels);
                targetVowelPair = tri ?? vowels[1];
            }

            var result = new StringBuilder(word);
            char targetChar = targetVowelPair.Value;
            int targetIdx = targetVowelPair.Key;
            char[] accentedArray;
            if (vowelMap.TryGetValue(targetChar, out accentedArray) && tone < accentedArray.Length)
                result[targetIdx] = accentedArray[tone];

            return result.ToString();
        }

        static char NormBase(char c)
        {
            char lc = char.ToLowerInvariant(c);
            switch (lc)
            {
                case 'ă': case 'â': return 'a';
                case 'ê': return 'E';
                case 'ô': return 'O';
                case 'ơ': return 'R';
                case 'ư': return 'U';
                default: return lc;
            }
        }

        static KeyValuePair<int, char>? FindTriphthongTargetIndex(List<KeyValuePair<int, char>> vowels)
        {
            var sb = new StringBuilder();
            foreach (var v in vowels) sb.Append(NormBase(v.Value));
            string cluster = sb.ToString();
            Func<int, KeyValuePair<int, char>?> at = n => (n >= 0 && n < vowels.Count) ? vowels[n] : (KeyValuePair<int, char>?)null;

            if (cluster.Length >= 3 && cluster[0] == 'u' && cluster[1] == 'y' && cluster[2] == 'E') return at(2);
            if (cluster.Length >= 3 && (cluster[0] == 'i' || cluster[0] == 'y') && cluster[1] == 'E' && cluster[2] == 'u') return at(1);
            if (cluster.Length >= 3 && cluster[0] == 'u' && cluster[1] == 'O' && cluster[2] == 'i') return at(1);
            if (cluster.Length >= 3 && cluster[0] == 'R' && (cluster[2] == 'i' || cluster[2] == 'u'))
            {
                int rPos = vowels.FindIndex(v => NormBase(v.Value) == 'R');
                if (rPos >= 0) return at(rPos);
            }
            if (cluster.Length >= 3 && cluster[0] == 'o' && cluster[1] == 'a'
                && (cluster[2] == 'i' || cluster[2] == 'y' || cluster[2] == 'o')) return at(1);
            if (cluster.Length >= 3 && cluster == "oeo") return at(1);
            if (cluster.Length >= 3 && cluster[0] == 'u' && cluster[1] == 'a' && cluster[2] == 'y') return at(1);

            return null;
        }
    }
}

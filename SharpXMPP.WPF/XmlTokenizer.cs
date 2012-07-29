using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpXMPP.WPF
{
    /*
     * this file implements a mostly correct XML tokenizer.  The token boundaries
     * have been chosen to match Visual Studio syntax highlighting, so a few of 
     * the boundaries are little weird.  (Especially comments) known issues:
     * 
     * Doesn't handle DTD's
     * mediocre handling of processing instructions <? ?> -- it won't crash, 
     *      but the token boundaries are wrong
     * Doesn't enforce correct XML
     * there's a few cases where it will die if given invalid XML
     * 
     * 
     * This tokenizer has been designed to be restartable, so you can tokenize
     * one line of XML at a time.
     */

    //enum TokenColors
    //{
    //    Punctuation, StringLiteral, ElementName, AttributeName, Comment, Normal
    //}

    enum XmlTokenKind : short
    {
        Open, // <
        Close,//>
        SelfClose,// />
        OpenClose,// </
        ElementName,
        ElementWhitespace,//whitespace between attributes
        AttributeName,
        Equals, // inside attribute
        AttributeValue, // attribute value
        CommentBegin, // <!--
        CommentText,
        CommentEnd, // -->
        Entity, // &gt;
        OpenProcessingInstruction, // <?
        CloseProcessingInstruction, // ?>
        CDataBegin, // <![CDATA[
        CDataEnd,// ]]>
        TextContent,
        //WhitespaceContent, // text content that's whitespace.  Space is embedded inside
        EOF, // end of file
    }

    // Used so you can restart the tokenizer for the next line of XML
    enum XmlTokenizerMode
    {
        InsideComment,
        InsideProcessingInstruction,
        AfterOpen,
        AfterAttributeName,
        AfterAttributeEquals,
        InsideElement, // after element name, before attribute or />
        OutsideElement,
        InsideCData,
    }

    struct XmlToken
    {
        public XmlTokenKind Kind;
        public short Length;
        public XmlToken(XmlTokenKind kind, int length)
        {
            Kind = kind;
            Length = (short)length;
        }
    }

    // XML tokenizer, tokens are designed to match Visual Studio syntax highlighting
    class XmlTokenizer
    {
        string input;
        int position = 0;
        XmlTokenizerMode mode = XmlTokenizerMode.OutsideElement;

        public static List<XmlToken> Tokenize(string input)
        {
            var mode = XmlTokenizerMode.OutsideElement;
            var tokenizer = new XmlTokenizer();
            return tokenizer.Tokenize(input, ref mode);
        }

        public List<XmlToken> Tokenize(string input, ref XmlTokenizerMode _mode)
        {
            this.input = input;
            this.mode = _mode;
            this.position = 0;
            var result = Tokenize();
            _mode = this.mode;
            return result;
        }

        private List<XmlToken> Tokenize()
        {
            var list = new List<XmlToken>();
            XmlToken token;
            do
            {
                var previousPosition = position;
                token = NextToken();
                var tokenText = input.Substring(previousPosition, token.Length);
                list.Add(token);
            } while (token.Kind != XmlTokenKind.EOF);

            var strings = TokensToStrings(list, input);

            return list;
        }

        private static List<string> TokensToStrings(IEnumerable<XmlToken> list, string input)
        {
            var output = new List<string>();
            var position = 0;
            foreach (var token in list)
            {
                output.Add(input.Substring(position, token.Length));
                position += token.Length;
            }
            return output;
        }

        // debugging function
        public string RemainingText
        {
            get { return input.Substring(position); }
        }

        private XmlToken NextToken()
        {
            if (position >= input.Length)
                return new XmlToken(XmlTokenKind.EOF, 0);

            XmlToken token;
            switch (mode)
            {
                case XmlTokenizerMode.AfterAttributeEquals:
                    token = TokenizeAttributeValue();
                    break;
                case XmlTokenizerMode.AfterAttributeName:
                    token = TokenizeSimple("=", XmlTokenKind.Equals, XmlTokenizerMode.AfterAttributeEquals);
                    break;
                case XmlTokenizerMode.AfterOpen:
                    token = TokenizeName(XmlTokenKind.ElementName, XmlTokenizerMode.InsideElement);
                    break;
                case XmlTokenizerMode.InsideCData:
                    token = TokenizeInsideCData();
                    break;
                case XmlTokenizerMode.InsideComment:
                    token = TokenizeInsideComment();
                    break;
                case XmlTokenizerMode.InsideElement:
                    token = TokenizeInsideElement();
                    break;
                case XmlTokenizerMode.InsideProcessingInstruction:
                    token = TokenizeInsideProcessingInstruction();
                    break;
                case XmlTokenizerMode.OutsideElement:
                    token = TokenizeOutsideElement();
                    break;
                default:
                    token = new XmlToken(XmlTokenKind.EOF, 0);
                    throw new Exception("missing case");
            }
            return token;
        }

        private static bool IsNameCharacter(char character)
        {
            // XML rule: Letter | Digit | '.' | '-' | '_' | ':' | CombiningChar | Extender
            bool result = char.IsLetterOrDigit(character)
            || character == '.' | character == '-' | character == '_' | character == ':';
            return result;
        }

        private XmlToken TokenizeAttributeValue()
        {
            Debug.Assert(mode == XmlTokenizerMode.AfterAttributeEquals);
            var closePosition = input.IndexOf(input[position], position + 1);
            var token = new XmlToken(XmlTokenKind.AttributeValue, closePosition + 1 - position);
            position = closePosition + 1;
            mode = XmlTokenizerMode.InsideElement;
            return token;
        }

        private XmlToken TokenizeName(XmlTokenKind kind, XmlTokenizerMode nextMode)
        {
            Debug.Assert(mode == XmlTokenizerMode.AfterOpen || mode == XmlTokenizerMode.InsideElement);
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (!IsNameCharacter(input[i]))
                {
                    break;
                }
            }
            var token = new XmlToken(kind, i - position);
            mode = nextMode;
            position = i;
            return token;
        }

        private XmlToken TokenizeElementWhitespace()
        {
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    break;
                }
            }
            var token = new XmlToken(XmlTokenKind.ElementWhitespace, i - position);
            position = i;
            return token;
        }

        private bool StartsWith(string text)
        {
            return position + text.Length <= input.Length && input.Substring(position, text.Length) == text;
        }

        private XmlToken TokenizeInsideElement()
        {
            return char.IsWhiteSpace(input[position])
                       ? TokenizeElementWhitespace()
                       : (StartsWith("/>")
                              ? TokenizeSimple("/>", XmlTokenKind.SelfClose, XmlTokenizerMode.OutsideElement)
                              : (StartsWith(">")
                                     ? TokenizeSimple(">", XmlTokenKind.Close, XmlTokenizerMode.OutsideElement)
                                     : TokenizeName(XmlTokenKind.AttributeName, XmlTokenizerMode.AfterAttributeName)));
        }


        //// We break on newlines because that makes it easier for us 
        //// to ignore the space after comments
        //private Token TokenizeWhitespaceContent()
        //{
        //    Debug.Assert(char.IsWhiteSpace(input[position]));
        //    bool sawNewline = false;
        //    int i;
        //    for (i = position; i < input.Length; i++) {
        //        if (!char.IsWhiteSpace(input[i])) {
        //            break;
        //        } else if (input[i] == '\n' || input[i] == '\r') {
        //            sawNewline = true;
        //        } else if (sawNewline) {
        //            break;
        //        }
        //    }
        //    Token token = new Token(TokenKind.WhitespaceContent, i - position);
        //    position = i;
        //    return token;
        //}


        private XmlToken TokenizeText()
        {
            Debug.Assert(input[position] != '<');
            Debug.Assert(input[position] != '&');
            Debug.Assert(mode == XmlTokenizerMode.OutsideElement);
            int i;
            for (i = position; i < input.Length; i++)
            {
                if (input[i] == '<' || input[i] == '&')
                {
                    break;
                }
            }
            var token = new XmlToken(XmlTokenKind.TextContent, i - position);
            position = i;
            return token;
        }

        private XmlToken TokenizeOutsideElement()
        {
            Debug.Assert(mode == XmlTokenizerMode.OutsideElement);
            if (position >= input.Length)
                return new XmlToken(XmlTokenKind.EOF, 0);

            switch (input[position])
            {
                case '<':
                    return TokenizeOpen();
                case '&':
                    return TokenizeEntity();
                default:
                    return TokenizeText();
            }
        }

        private XmlToken TokenizeSimple(string text, XmlTokenKind kind, XmlTokenizerMode nextMode)
        {
            var token = new XmlToken(kind, text.Length);
            position += text.Length;
            mode = nextMode;
            return token;
        }

        private XmlToken TokenizeOpen()
        {
            Debug.Assert(input[position] == '<');
            return StartsWith("<!--")
                       ? TokenizeSimple("<!--", XmlTokenKind.CommentBegin, XmlTokenizerMode.InsideComment)
                       : (StartsWith("<![CDATA[")
                              ? TokenizeSimple("<![CDATA[", XmlTokenKind.CDataBegin, XmlTokenizerMode.InsideCData)
                              : (StartsWith("<?")
                                     ? TokenizeSimple("<?", XmlTokenKind.OpenProcessingInstruction,
                                                      XmlTokenizerMode.InsideProcessingInstruction)
                                     : (StartsWith("</")
                                            ? TokenizeSimple("</", XmlTokenKind.OpenClose, XmlTokenizerMode.AfterOpen)
                                            : TokenizeSimple("<", XmlTokenKind.Open, XmlTokenizerMode.AfterOpen))));
        }

        private XmlToken TokenizeEntity()
        {
            Debug.Assert(mode == XmlTokenizerMode.OutsideElement);
            Debug.Assert(input[position] == '&');
            var token = new XmlToken(XmlTokenKind.Entity, input.IndexOf(';', position) - position);
            position += token.Length;
            return token;
        }

        private XmlToken TokenizeInsideProcessingInstruction()
        {
            Debug.Assert(mode == XmlTokenizerMode.InsideProcessingInstruction);
            var tokenend = input.IndexOf("?>", position);
            if (position == tokenend)
            {
                position += "?>".Length;
                mode = XmlTokenizerMode.OutsideElement;
                return new XmlToken(XmlTokenKind.CloseProcessingInstruction, "?>".Length);
            }
            var token = new XmlToken(XmlTokenKind.TextContent, tokenend - position);
            position = tokenend;
            return token;
        }

        private XmlToken TokenizeInsideCData()
        {
            Debug.Assert(mode == XmlTokenizerMode.InsideCData);
            var tokenend = input.IndexOf("]]>", position);
            if (position == tokenend)
            {
                position += "]]>".Length;
                mode = XmlTokenizerMode.OutsideElement;
                return new XmlToken(XmlTokenKind.CDataEnd, "]]>".Length);
            }
            var token = new XmlToken(XmlTokenKind.TextContent, tokenend - position);
            position = tokenend;
            return token;
        }


        private XmlToken TokenizeInsideComment()
        {
            Debug.Assert(mode == XmlTokenizerMode.InsideComment);
            var tokenend = input.IndexOf("-->", position);
            if (position == tokenend)
            {
                position += "-->".Length;
                mode = XmlTokenizerMode.OutsideElement;
                return new XmlToken(XmlTokenKind.CommentEnd, "-->".Length);
            }
            var token = new XmlToken(XmlTokenKind.CommentText, tokenend - position);
            position = tokenend;
            return token;
        }
    }
}
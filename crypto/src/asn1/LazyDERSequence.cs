using System;
using System.Collections;
using System.Diagnostics;

namespace Org.BouncyCastle.Asn1
{
	internal class LazyDerSequence
		: DerSequence
	{
		private byte[] encoded;

        internal LazyDerSequence(
			byte[] encoded)
		{
			this.encoded = encoded;
		}

		private void Parse()
		{
			lock (this)
			{
                if (null != encoded)
                {
                    Asn1InputStream e = new LazyAsn1InputStream(encoded);
                    Asn1EncodableVector v = e.ReadVector();

                    this.elements = v.TakeElements();
                    this.encoded = null;
                }
            }
		}

		public override Asn1Encodable this[int index]
		{
			get
			{
				Parse();

				return base[index];
			}
		}

		public override IEnumerator GetEnumerator()
		{
			Parse();

			return base.GetEnumerator();
		}

		public override int Count
		{
			get
			{
				Parse();

				return base.Count;
			}
		}

		internal override void Encode(Asn1OutputStream asn1Out)
		{
			lock (this)
			{
				if (encoded == null)
				{
					base.Encode(asn1Out);
				}
				else
				{
					asn1Out.WriteEncoded(Asn1Tags.Sequence | Asn1Tags.Constructed, encoded);
				}
			}
		}
	}
}

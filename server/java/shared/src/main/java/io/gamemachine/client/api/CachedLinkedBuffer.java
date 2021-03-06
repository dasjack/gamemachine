package io.gamemachine.client.api;

import static io.protostuff.LinkedBuffer.allocate;

import io.protostuff.LinkedBuffer;

public class CachedLinkedBuffer {

	static int bufferSize = 2048;

	private static final ThreadLocal<LinkedBuffer> localBuffer = new ThreadLocal<LinkedBuffer>() {
		@Override
		protected LinkedBuffer initialValue() {
			return allocate(bufferSize);
		}
	};

	public static LinkedBuffer get() {
		return localBuffer.get();
	}
}
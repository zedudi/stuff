import tensorflow as tf

class BayerRg8ToRgb:
    # Tensorflow based converter for images with Bayer RG 8-bit pixelformat to RGB.
    # The kernels are based on the following whitepaper:
    # http://web.stanford.edu/class/ee367/reading/Demosaicing_ICASSP04.pdf
    def __init__(self, h, w):
        self.size = (h, w)
        self.h = h
        self.w = w
        self.k1 = tf.constant([[0, 0, 0, 0, 0],
                               [0, 0, 0, 0, 0],
                               [0, 0, 8, 0, 0],
                               [0, 0, 0, 0, 0],
                               [0, 0, 0, 0, 0]], dtype = tf.float32)

        self.k2 = tf.constant([[0, 0, 0.5, 0, 0],
                               [0, -1, 0, -1, 0],
                               [-1, 4, 5, 4, -1],
                               [0, -1, 0, -1, 0],
                               [0, 0, 0.5, 0, 0]], dtype = tf.float32)

        self.k3 = tf.constant([[0, 0, -1, 0, 0],
                               [0, -1, 4, -1, 0],
                               [0.5, 0, 5, 0, 0.5],
                               [0, -1, 4, -1, 0],
                               [0, 0, -1, 0, 0]], dtype = tf.float32)

        self.k4 = tf.constant([[0, 0, -1.5, 0, 0],
                               [0, 2, 0, 2, 0],
                               [-1.5, 0, 6, 0, -1.5],
                               [0, 2, 0, 2, 0],
                               [0, 0, -1.5, 0, 0]], dtype = tf.float32)

        self.k5 = tf.constant([[0, 0, -1, 0, 0],
                               [0, 0, 2, 0, 0],
                               [-1, 2, 4, 2, -1],
                               [0, 0, 2, 0, 0],
                               [0, 0, -1, 0, 0]], dtype = tf.float32)

    def convert(self, raw, batch=1):
        raw = tf.constant(value=raw, shape=(batch, self.h, self.w, 1), dtype=tf.uint8)
        raw = tf.cast(raw, dtype=tf.float32)

        rr = tf.reshape(tf.pad(self.k1, [[0, 1], [0, 1]]), shape=(6, 6, 1, 1))
        gr = tf.reshape(tf.pad(self.k5, [[0, 1], [0, 1]]), shape=(6, 6, 1, 1))
        br = tf.reshape(tf.pad(self.k4, [[0, 1], [0, 1]]), shape=(6, 6, 1, 1))

        rg1 = tf.reshape(tf.pad(self.k2, [[0, 1], [1, 0]]), shape=(6, 6, 1, 1))
        gg1 = tf.reshape(tf.pad(self.k5, [[0, 1], [1, 0]]), shape=(6, 6, 1, 1))
        bg1 = tf.reshape(tf.pad(self.k3, [[0, 1], [1, 0]]), shape=(6, 6, 1, 1))

        rg2 = tf.reshape(tf.pad(self.k3, [[1, 0], [0, 1]]), shape=(6, 6, 1, 1))
        gg2 = tf.reshape(tf.pad(self.k5, [[1, 0], [0, 1]]), shape=(6, 6, 1, 1))
        bg2 = tf.reshape(tf.pad(self.k2, [[1, 0], [0, 1]]), shape=(6, 6, 1, 1))

        rb = tf.reshape(tf.pad(self.k4, [[1, 0], [1, 0]]), shape=(6, 6, 1, 1))
        gb = tf.reshape(tf.pad(self.k5, [[1, 0], [1, 0]]), shape=(6, 6, 1, 1))
        bb = tf.reshape(tf.pad(self.k1, [[1, 0], [1, 0]]), shape=(6, 6, 1, 1))

        filt = tf.concat([rr, gr, br, rg1, gg1, bg1, rg2, gg2, bg2, rb, gb, bb], axis=-1)
        res = tf.nn.conv2d(input=raw, filters=filt, strides=[1, 2, 2, 1], padding='SAME') / 8

        res = tf.reshape(res, res.shape[0:3] + (2, 2, 3))
        res = tf.transpose(res, [0, 1, 3, 2, 4, 5])
        res = tf.reshape(res, (res.shape[0], res.shape[1] * res.shape[2], res.shape[3] * res.shape[4], 3))

        return res
